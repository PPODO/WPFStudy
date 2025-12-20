#include <iostream>
#include <thread>
#include <vector>
#include <mutex>
#include <map>
#include <WinSock2.h>
#pragma comment(lib, "ws2_32.lib")
#include "Protocol.h"

#define RECV_BUFF_SIZE 2048

std::mutex g_ClientContainerMutex;
std::map<SOCKET, int> g_ClientContainer;

void RecvThreadFunction(SOCKET hListenSocket)
{
	char messageBuffer[RECV_BUFF_SIZE] = { "\0" };
	int remain_bytes = 0;

	while (true)
	{
		const int recv_bytes = recv(hListenSocket, messageBuffer + remain_bytes, RECV_BUFF_SIZE - remain_bytes, 0);
		if (recv_bytes <= 0)
		{
			std::cout << hListenSocket << " IS CLOSED!\n";
			goto close;
		}

		remain_bytes += recv_bytes;

		while (true)
		{
			if (remain_bytes < sizeof(BasicProtocol))
				break;

			auto basicProtocol = reinterpret_cast<BasicProtocol*>(messageBuffer);
			if (!basicProtocol)
				goto error;
			
			if (remain_bytes < basicProtocol->_totalPacketSize)
				break;

			char SendBuffer[RECV_BUFF_SIZE] = { "\0" };
			switch (basicProtocol->_messageType)
			{
			case MessageType::MSG_REQUEST_SESSION_LIST:
			{
				auto REQUEST_SESSION_LIST = reinterpret_cast<BasicProtocol*>(messageBuffer);
				if (!REQUEST_SESSION_LIST)
					goto error;

				{
					NOTICE_SESSION_LIST session;

					session.session_id = 1;
					session.joined_user_count = 5;
					session.max_user_count = 10;

					std::string session_name = "HelloWorld";
					session.session_name.resize(session_name.length() + 1);
					strcpy_s((char*)&session.session_name.front(), session_name.length() + 1, session_name.c_str());

					session.session_name_length = session.session_name.size();
					session._totalPacketSize = session.GetSize();

					if (session.Parse(SendBuffer))
						send(hListenSocket, SendBuffer, session.GetSize(), 0);
				}
			}
			break;
			}

			remain_bytes -= basicProtocol->_totalPacketSize;
		}
	}

error:
	std::cout << hListenSocket << "RECV ERROR\n\n";

close:

	g_ClientContainerMutex.lock();

	auto find = g_ClientContainer.find(hListenSocket);
	if (find != g_ClientContainer.cend())
	{
		closesocket(find->first);
		g_ClientContainer.erase(find);
	}

	g_ClientContainerMutex.unlock();
}

int main()
{
	WSADATA winData;
	WSAStartup(MAKEWORD(2, 2), &winData);

	SOCKET hSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	sockaddr_in sockAddress;
	sockAddress.sin_family = AF_INET;
	sockAddress.sin_port = htons(3550);
	sockAddress.sin_addr.s_addr = inet_addr("127.0.0.1");

	int addr_size = sizeof(sockaddr);
	std::vector<std::thread> recv_thread_list;

	if (bind(hSocket, reinterpret_cast<sockaddr*>(&sockAddress), sizeof(sockaddr_in)) == SOCKET_ERROR)
		goto shutdown;

	if (listen(hSocket, SOMAXCONN) == SOCKET_ERROR)
		goto shutdown;

	sockaddr client_addr;
	while (true)
	{
		SOCKET hClientSocket = accept(hSocket, &client_addr, &addr_size);
		if (hClientSocket == SOCKET_ERROR)
			continue;

		std::cout << "NEW CLIENT ACCEPTED! RECV THREAD CREATED!\n";

		g_ClientContainerMutex.lock();
		g_ClientContainer.emplace(hClientSocket, 0);
		g_ClientContainerMutex.unlock();

		recv_thread_list.emplace_back(std::thread(RecvThreadFunction, hClientSocket));
	}

	for (auto& thread : recv_thread_list)
		thread.join();

shutdown:
	closesocket(hSocket);
	WSACleanup();
	return 0;
}