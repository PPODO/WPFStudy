#pragma once

enum class MessageType
{
	MSG_NONE = 0,
	MSG_REQUEST_SESSION_LIST = 1,
	MSG_NOTICE_SESSION = 2,
	MSG_RESPONSE_SESSION_LIST = 3,

	MSG_REQUEST_CREATE_SESSION = 4,
	MSG_RESPONSE_CREATE_SESSION = 5,

	MSG_REQUEST_JOIN_SESSION = 6,
	MSG_RESPONSE_JOIN_SESSION = 7,
	MSG_NOTICE_JOIN_SESSION = 8,

	MSG_REQUEST_LEAVE_SESSION = 9,
	MSG_RESPONSE_LEAVE_SESSION = 10,
	MSG_NOTICE_LEAVE_SESSION = 11,

	MSG_REQUEST_CHAT_MESSAGE = 12,
	MSG_NOTICE_CHAT_MESSAGE = 13,
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

public:
	size_t GetSize() const { return sizeof(BasicProtocol) + sizeof(uint32_t) + session_name.length(); }

};

struct RESPONSE_CREATE_SESSION : BasicProtocol
{
public:
	uint32_t session_id;

	uint16_t feedback;

public:
	RESPONSE_CREATE_SESSION()
		: BasicProtocol(MessageType::MSG_RESPONSE_CREATE_SESSION, sizeof(RESPONSE_CREATE_SESSION))
		, feedback()
		, session_id()
	{
	}

public:
	bool Parse(char* buffer)
	{
		if (!buffer) return false;

		memcpy(buffer, &_messageType, sizeof(_messageType));
		memcpy(buffer + sizeof(_messageType), &_totalPacketSize, sizeof(_totalPacketSize));

		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize), &session_id, sizeof(session_id));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id), &feedback, sizeof(feedback));

		return true;
	}

	uint32_t GetSize()
	{
		return sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(session_id) + sizeof(feedback);
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

struct REQUEST_JOIN_SESSION : BasicProtocol
{
public:
	uint32_t session_id;

	uint16_t joined_user_nickname_length;
	std::string joined_user_nickname;

public:
	REQUEST_JOIN_SESSION()
		: BasicProtocol(MessageType::MSG_REQUEST_JOIN_SESSION, sizeof(REQUEST_JOIN_SESSION))
		, session_id()
		, joined_user_nickname_length()
		, joined_user_nickname()
	{
	}

public:
	size_t GetSize() const { return sizeof(BasicProtocol) + sizeof(uint32_t) + sizeof(uint16_t) + joined_user_nickname.length(); }

};

struct RESPONSE_JOIN_SESSION : BasicProtocol
{
public:
	uint16_t feedback;
	uint32_t session_id;

public:
	RESPONSE_JOIN_SESSION()
		: BasicProtocol(MessageType::MSG_RESPONSE_JOIN_SESSION, sizeof(RESPONSE_JOIN_SESSION))
		, feedback()
		, session_id()
	{
	}

public:
	bool Parse(char* buffer)
	{
		if (!buffer) return false;

		memcpy(buffer, &_messageType, sizeof(_messageType));
		memcpy(buffer + sizeof(_messageType), &_totalPacketSize, sizeof(_totalPacketSize));

		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize), &feedback, sizeof(feedback));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(feedback), &session_id, sizeof(session_id));

		return true;
	}

	uint32_t GetSize()
	{
		return sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(feedback) + sizeof(session_id);
	}

};

struct NOTICE_JOIN_SESSION : BasicProtocol
{
public:
	uint16_t joined_user_nickname_length;
	std::string joined_user_nickname;

public:
	NOTICE_JOIN_SESSION()
		: BasicProtocol(MessageType::MSG_NOTICE_JOIN_SESSION, sizeof(NOTICE_JOIN_SESSION))
		, joined_user_nickname_length()
		, joined_user_nickname()
	{
	}

public:
	bool Parse(char* buffer)
	{
		if (!buffer) return false;

		memcpy(buffer, &_messageType, sizeof(_messageType));
		memcpy(buffer + sizeof(_messageType), &_totalPacketSize, sizeof(_totalPacketSize));

		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize), &joined_user_nickname_length, sizeof(joined_user_nickname_length));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(joined_user_nickname_length), joined_user_nickname.c_str(), joined_user_nickname.length());

		return true;
	}

	size_t GetSize() const { return sizeof(BasicProtocol) + sizeof(uint16_t) + joined_user_nickname.length(); }

};

struct REQUEST_CHAT_MESSAGE : public BasicProtocol
{
public:
	uint32_t session_id;

	uint16_t chat_message_length;
	std::string chat_message;

public:
	REQUEST_CHAT_MESSAGE()
		: BasicProtocol(MessageType::MSG_REQUEST_CHAT_MESSAGE, sizeof(REQUEST_CHAT_MESSAGE))
		, session_id()
		, chat_message_length()
		, chat_message()
	{
	}

public:
	size_t GetSize() const { return sizeof(BasicProtocol) + sizeof(uint32_t) + sizeof(uint16_t) + chat_message.length(); }

};

struct NOTICE_CHAT_MESSAGE : public BasicProtocol
{
public:
	uint16_t joined_user_nickname_length;
	std::string joined_user_nickname;

	uint16_t chat_message_length;
	std::string chat_message;

public:
	NOTICE_CHAT_MESSAGE()
		: BasicProtocol(MessageType::MSG_NOTICE_CHAT_MESSAGE, sizeof(NOTICE_CHAT_MESSAGE))
		, joined_user_nickname_length()
		, joined_user_nickname()
		, chat_message_length()
		, chat_message()
	{
	}

public:
	bool Parse(char* buffer)
	{
		if (!buffer) return false;

		memcpy(buffer, &_messageType, sizeof(_messageType));
		memcpy(buffer + sizeof(_messageType), &_totalPacketSize, sizeof(_totalPacketSize));

		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize), &joined_user_nickname_length, sizeof(joined_user_nickname_length));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(joined_user_nickname_length), joined_user_nickname.c_str(), joined_user_nickname.length());

		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(joined_user_nickname_length) + joined_user_nickname.length(), &chat_message_length, sizeof(chat_message_length));
		memcpy(buffer + sizeof(_messageType) + sizeof(_totalPacketSize) + sizeof(joined_user_nickname_length) + joined_user_nickname.length() + sizeof(chat_message_length), chat_message.c_str(), chat_message.length());

		return true;
	}

	size_t GetSize() const { return sizeof(BasicProtocol) + sizeof(uint16_t) + joined_user_nickname.length() + sizeof(uint16_t) + chat_message.length(); }


};