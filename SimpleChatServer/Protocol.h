#pragma once

enum MSG
{
    MSG_NONE = 0,
    MSG_REQUEST_SESSION_LIST = 1,
    MSG_NOTICE_SESSION_LIST = 2,
    MSG_RESPONSE_SESSION_LIST = 3,

};

struct BasicProtocol
{
public:
    MSG _messageType;
    int _totalPacketSize;

public:
    BasicProtocol(MSG messageType = MSG_NONE, int totalPacketSize = 0)
    {
        _messageType = messageType;
        _totalPacketSize = totalPacketSize;
    }

};

struct NOTICE_SESSION_LIST : BasicProtocol
{
public:



public:



};