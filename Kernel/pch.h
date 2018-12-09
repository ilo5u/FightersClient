#pragma once

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif // !_CRT_SECURE_NO_WARNINGS

#ifndef _WINSOCK_DEPRECATED_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#endif // !_WINSOCK_DEPRECATED_NO_WARNINGS

#include <collection.h>
#include <ppltasks.h>
#include <exception>
#include <queue>
#include <mutex>
#include <iostream>

#include <WinSock2.h>
#include <WS2tcpip.h>

#include "IO.h"
#include "RandomGenerator.hpp"
#include "Player.h"
#include "Pokemen.h"
#include "NetIO.h"