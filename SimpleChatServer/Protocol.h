#pragma once

enum class MessageType
{
	MSG_NONE = 0,
	MSG_REQUEST_SESSION_LIST = 1,
	MSG_NOTICE_SESSION = 2,
	MSG_RESPONSE_SESSION_LIST = 3,

	MSG_REQUEST_CREATE_SESSION = 4,
	MSG_RESPONSE_CREATE_SESSION = 5,

};

struct BasicProtocol
{
public:
	MessageType _messageType;
	int _totalPacketSize;

public:
	BasicProtocol(MessageType messageType = MessageType::MSG_NONE, int totalPacketSize = 0)
	{
		_messageType = messageType;
		_totalPacketSize = totalPacketSize;
	}

};

struct REQUEST_SESSION_LIST : BasicProtocol
{
public:
	REQUEST_SESSION_LIST()
		: BasicProtocol(MessageType::MSG_REQUEST_SESSION_LIST, sizeof(REQUEST_SESSION_LIST))
	{
	}

};

struct REQUEST_CREATE_SESSION : BasicProtocol
{
public:
	uint16_t max_user_count;

	uint16_t session_name_length;
	std::string session_name;

public:
	REQUEST_CREATE_SESSION()
		: BasicProtocol(MessageType::MSG_REQUEST_CREATE_SESSION, sizeof(REQUEST_CREATE_SESSION))
		, max_user_count()
		, session_name_length()
		, session_name()
	{
	}
};

struct NOTICE_SESSION : BasicProtocol
{
public:
	uint32_t session_id;

	uint16_t joined_user_count;
	uint16_t max_user_count;

	uint16_t session_name_length;
	std::string session_name;

public:
	NOTICE_SESSION()
		: BasicProtocol(MessageType::MSG_NOTICE_SESSION, sizeof(NOTICE_SESSION))
		, session_id(0)
		, joined_user_count(0)
		, max_user_count(0)
		, session_name_length(0)
		, session_name()
	{
	}

public:
	bool Parse(char* buffer)
	{
		if (!buffer || session_name.empty()) return false;

		memcpy(buffer, &_messageType, sizeof(_messageType));
		memcpy(buffer + sizeof(_messageType), &_totalPacketSize, sizeof(_totalPacketSize));

		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize), &session_id, sizeof(session_id));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id), &joined_user_count, sizeof(joined_user_count));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id) + sizeof(joined_user_count), &max_user_count, sizeof(max_user_count));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id) + sizeof(joined_user_count) + sizeof(max_user_count), &session_name_length, sizeof(session_name_length));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id) + sizeof(joined_user_count) + sizeof(max_user_count) + sizeof(session_name_length), session_name.c_str(), session_name.length());

		return true;
	}

	uint32_t GetSize()
	{
		return sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id) + sizeof(joined_user_count) + sizeof(max_user_count) + sizeof(session_name_length) + session_name.length();
	}

};

