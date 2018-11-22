#include "pch.h"
#include "Background.h"

using namespace Platform;
namespace Kernel
{
	std::string WStringToString(const wchar_t* wstr)
	{
		int iLen = WideCharToMultiByte(CP_ACP, 0, wstr,
			std::wcslen(wstr), NULL, 0, NULL, NULL);

		if (iLen <= 0)
			return { "" };

		char *szDst = new char[iLen + 1];
		if (szDst == nullptr)
			return { "" };

		WideCharToMultiByte(CP_ACP, 0, wstr,
			std::wcslen(wstr), szDst, iLen, NULL, NULL);
		szDst[iLen] = '\0';

		std::string str{ szDst };
		delete[] szDst;

		return str;
	}

	std::wstring StringToWString(const char* str)
	{
		int iLen = MultiByteToWideChar(CP_ACP, 0, str,
			std::strlen(str), NULL, 0);

		if (iLen <= 0)
			return { L"" };

		wchar_t *wszDst = new wchar_t[iLen + 1];
		if (wszDst == nullptr)
			return { L"" };

		MultiByteToWideChar(CP_ACP, 0, str,
			std::strlen(str), wszDst, iLen);
		wszDst[iLen] = L'\0';

		if (wszDst[0] == 0xFEFF)
			for (int i = 0; i < iLen; ++i)
				wszDst[i] = wszDst[i + 1];

		std::wstring wstr{ wszDst };
		delete wszDst;
		wszDst = nullptr;

		return wstr;
	}

	Pokemen::Pokemen(int type, int level) :
		instance(static_cast<::Pokemen::PokemenType>(type), level)
	{
	}

	Pokemen::~Pokemen()
	{
	}

	static ::Pokemen::Pokemen aiPlayer(::Pokemen::PokemenType::DEFAULT, 1);
	void Pokemen::SetAIPlayer()
	{
		aiPlayer = instance;
	}

	Property Pokemen::GetProperty()
	{
		return Property{
			instance.GetId(),
			(int)instance.GetType(),
			ref new Platform::String(
				StringToWString(instance.GetName().c_str()).c_str()
			),
			instance.GetHpoints(),
			instance.GetAttack(),
			instance.GetDefense(),
			instance.GetAgility(),
			instance.GetInterval(),
			instance.GetCritical(),
			instance.GetHitratio(),
			instance.GetParryratio(),
			instance.GetCareer(),
			instance.GetExp(),
			instance.GetLevel()
		};
	}

	Core::Core() :
		netDriver(), stage(), pokemens()
	{
	}

	Core::~Core()
	{
	}

	void Core::SendMessage(Message msg)
	{
		Packet sendPacket;
		switch (msg.type)
		{
		case MsgType::LOGIN_REQUEST:
			sendPacket.type = PacketType::LOGIN_REQUEST;
			break;

		case MsgType::LOGON_REQUEST:
			sendPacket.type = PacketType::LOGON_REQUEST;
			break;

		case MsgType::LOGOUT:
			sendPacket.type = PacketType::LOGOUT;
			break;

		default:
			break;
		}
		std::memcpy(
			(LPCH)sendPacket.data,
			WStringToString(msg.data->Data()).c_str(),
			msg.data->Length()
		);
		netDriver.SendPacket(sendPacket);
	}

	Message Core::ReadOfflineMessage()
	{
		return Message();
	}

	Message Core::ReadOnlineMessage()
	{
		try
		{
			Packet recvPacket = netDriver.ReadPacket();
			switch (recvPacket.type)
			{
			case PacketType::LOGIN_SUCCESS:
				return {
					MsgType::LOGIN_SUCCESS,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			case PacketType::LOGIN_FAILED:
				return {
					MsgType::LOGIN_FAILED,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			case PacketType::LOGON_SUCCESS:
				return {
					MsgType::LOGON_SUCCESS,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			case PacketType::LOGON_FAILED:
				return {
					MsgType::LOGON_FAILED,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			case PacketType::UPDATE_ONLINE_USERS:
				return {
					MsgType::UPDATE_ONLINE_USERS,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			case PacketType::UPDATE_POKEMENS:
				return {
					MsgType::UPDATE_POKEMENS,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			case PacketType::SET_ONLINE_USERS:
				return {
					MsgType::SET_ONLINE_USERS,
					ref new Platform::String(StringToWString(recvPacket.data).c_str())
				};

			default:
				return { };
			}
		}
		catch (const std::exception& e)
		{
			OutputDebugStringA(e.what());
			return { };
		}
	}

	bool Core::StartConnection()
	{
		try
		{
			if (netDriver.IsConnected())
				return false;
			return netDriver.Connect();
		}
		catch (const std::exception& e)
		{
			OutputDebugStringA(e.what());
			return false;
		}
	}

	bool Core::CloseConnection()
	{
		return netDriver.Disconnect();
	}

	bool Core::IsConnected()
	{
		return netDriver.IsConnected();
	}

	void Core::SetBattlePlayersAndType(int pokemenId, Kernel::Pokemen^ ai, int type)
	{
		ai->SetAIPlayer();
		for (Pokemens::const_iterator it = pokemens.begin();
			it != pokemens.end(); ++it)
		{
			if ((*it)->GetId() == pokemenId)
			{
				stage.SetPlayers(*(*it), aiPlayer);
				break;
			}
		}
	}

	void Core::SetBattleOn()
	{
		stage.GoOn();
	}

	void Core::SetBattlePasue()
	{
		stage.Pause();
	}

	void Core::ShutdownBattle()
	{
		stage.Clear();
	}

	bool Core::IsBattleRunning()
	{
		return stage.IsRunning();
	}

}