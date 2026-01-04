#include <iostream>
#include <thread>
#include <vector>
#include <mutex>
#include <map>
#include <WinSock2.h>
#pragma comment(lib, "ws2_32.lib")
#include "Protocol.h"

#define RECV_BUFF_SIZE 2048

struct ChatSessionInfo
{
public:
	uint32_t session_unique_id;

	std::string session_name;
	
	std::vector<SOCKET> joined_user_socket_handler;
	uint16_t max_user_count;

public:
	ChatSessionInfo()
		: session_unique_id()
		, session_name()
		, max_user_count()
	{
	}

};

std::mutex g_ClientContainerMutex;
std::map<SOCKET, int> g_ClientContainer;

std::mutex g_ChatSessionMutex;
std::atomic<uint32_t> g_ChatSessionIncreaseIndex;
std::map<uint32_t, ChatSessionInfo> g_ChatSessionContainer;

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

				g_ChatSessionMutex.lock();

				for (const auto& session : g_ChatSessionContainer)
				{
					NOTICE_SESSION notice_session;

					notice_session.session_id = session.second.session_unique_id;
					notice_session.joined_user_count = session.second.joined_user_socket_handler.size();
					notice_session.max_user_count = session.second.max_user_count;
					notice_session.session_name = session.second.session_name;
					notice_session.session_name_length = session.second.session_name.size();
					notice_session._totalPacketSize = notice_session.GetSize();

					if (notice_session.Parse(SendBuffer))
						send(hListenSocket, SendBuffer, notice_session.GetSize(), 0);
				}

				g_ChatSessionMutex.unlock();
			}
			break;
			case MessageType::MSG_REQUEST_CREATE_SESSION:
			{
				auto packet = reinterpret_cast<REQUEST_CREATE_SESSION*>(messageBuffer);
				if (!packet)
					goto error;

				g_ChatSessionMutex.lock();

				ChatSessionInfo new_chat_session_info;
				new_chat_session_info.session_unique_id = ++g_ChatSessionIncreaseIndex;
				new_chat_session_info.session_name = messageBuffer + packet->GetSize();
				new_chat_session_info.max_user_count = packet->max_user_count;

				g_ChatSessionContainer.emplace(new_chat_session_info.session_unique_id, new_chat_session_info);

				g_ChatSessionMutex.unlock();

				// response
				{
					RESPONSE_CREATE_SESSION response;

					response.feedback = 0;
					response.session_id = new_chat_session_info.session_unique_id;
					response._totalPacketSize = response.GetSize();

					if (response.Parse(SendBuffer))
						send(hListenSocket, SendBuffer, response.GetSize(), 0);
				}

				// notice other clients
				{
					NOTICE_SESSION session;

					session.session_id = new_chat_session_info.session_unique_id;
					session.joined_user_count = new_chat_session_info.joined_user_socket_handler.size();
					session.max_user_count = new_chat_session_info.max_user_count;
					session.session_name = new_chat_session_info.session_name;
					session.session_name_length = new_chat_session_info.session_name.size();
					session._totalPacketSize = session.GetSize();

					if (session.Parse(SendBuffer))
					{
						g_ClientContainerMutex.lock();

						for (const auto& client : g_ClientContainer)
						{
							if (client.first != hListenSocket)
								send(client.first, SendBuffer, session.GetSize(), 0);
						}

						g_ClientContainerMutex.unlock();
					}
				}
			}
			break;
			case MessageType::MSG_REQUEST_JOIN_SESSION:
			{
				auto packet = reinterpret_cast<REQUEST_JOIN_SESSION*>(messageBuffer);
				if (!packet)
					goto error;

				bool succ = false;

				g_ChatSessionMutex.lock();

				auto find = g_ChatSessionContainer.find(packet->session_id);
				if (find == g_ChatSessionContainer.cend())
				{
					g_ChatSessionMutex.unlock();
					break;
				}

				const std::string joined_user_nickname = messageBuffer + packet->GetSize();

				if ((find->second.joined_user_socket_handler.size() + 1) <= find->second.max_user_count)
					succ = true;

				// notice
				if (succ)
				{
					NOTICE_JOIN_SESSION join_session;

					join_session.joined_user_nickname_length = joined_user_nickname.length();
					join_session.joined_user_nickname = joined_user_nickname;
					join_session._totalPacketSize = join_session.GetSize();

					if (join_session.Parse(SendBuffer))
					{
						for (const auto& client : find->second.joined_user_socket_handler)
							send(client, SendBuffer, join_session.GetSize(), 0);
					}

					find->second.joined_user_socket_handler.emplace_back(hListenSocket);
				}

				g_ChatSessionMutex.unlock();

				// notice lobby player
				{


				}

				// response
				{
					RESPONSE_JOIN_SESSION response;

					response.feedback = (!succ);
					response.session_id = packet->session_id;
					response._totalPacketSize = response.GetSize();

					if (response.Parse(SendBuffer))
						send(hListenSocket, SendBuffer, response.GetSize(), 0);
				}
			}
			break;
			case MessageType::MSG_REQUEST_CHAT_MESSAGE:
			{
				auto packet = reinterpret_cast<REQUEST_CHAT_MESSAGE*>(messageBuffer);
				if (!packet)
					goto error;

				const std::string chat = messageBuffer + packet->GetSize();

				g_ChatSessionMutex.lock();

				const auto find = g_ChatSessionContainer.find(packet->session_id);
				if (find == g_ChatSessionContainer.cend())
				{
					goto close;
					g_ChatSessionMutex.unlock();
				}

				g_ChatSessionMutex.unlock();

				const auto client_find = std::find_if(
					find->second.joined_user_socket_handler.begin(), find->second.joined_user_socket_handler.end(),
					[&hListenSocket](const SOCKET rhs) { return rhs == hListenSocket; });

				if (client_find == find->second.joined_user_socket_handler.cend())
					goto close;

				{
					NOTICE_CHAT_MESSAGE notice_packet;

					notice_packet.joined_user_nickname_length = 3;
					notice_packet.joined_user_nickname = "YYY"; // todo

					notice_packet.chat_message_length = chat.length();
					notice_packet.chat_message = chat;
					notice_packet._totalPacketSize = notice_packet.GetSize();

					if (notice_packet.Parse(SendBuffer))
					{
						for (const auto& client : find->second.joined_user_socket_handler)
							send(client, SendBuffer, notice_packet.GetSize(), 0);
					}
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

#include <concurrent_priority_queue.h>

struct Unit
{
public:
	int val;

public:
	Unit(const int _val = 0)
		: val(_val)
	{
	}

};

struct UnitCompareFunction
{
	bool operator()(const Unit& lhs, const Unit& rhs)
	{
		return lhs.val < rhs.val;
	}
};

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