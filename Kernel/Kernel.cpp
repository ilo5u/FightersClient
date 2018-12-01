#include "pch.h"
#include "Kernel.h"

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

	Pokemen::Pokemen(int level) :
		instance(::Pokemen::PokemenType::DEFAULT, level)
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
		if (!netDriver.IsConnected())
			return;

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

		if (sendPacket.type == PacketType::LOGOUT)
			netDriver.Disconnect();
	}

	Message Core::ReadOfflineMessage()
	{
		if (stage.IsRunning()
			|| stage.ReadyForRead())
		{
			::Pokemen::BattleMessage msg = stage.ReadMessage();
			switch (msg.type)
			{
			case BattleMessage::Type::DISPLAY:
				return Message{
					MsgType::PVE_MESSAGE,
					ref new Platform::String(StringToWString(msg.options.c_str()).c_str())
				};
				
			case BattleMessage::Type::RESULT:
			{
				/* 向服务器回传战斗结果 */
				Packet packet;
				packet.type = PacketType::PVE_RESULT;
				sprintf(packet.data, "%s", msg.options.c_str());

				/* 升级对应小精灵 */
				int pokemenId;
				int raiseExp = 0;
				if (msg.options[0] == 'F')
					sscanf(msg.options.c_str(), "F\n%d\n%d\n", &pokemenId, &raiseExp);
				else if (msg.options[0] == 'S')
					sscanf(msg.options.c_str(), "S\n%d\n%d\n", &pokemenId, &raiseExp);
				Pokemens::iterator it = std::find_if(
					pokemens.begin(), pokemens.end(), [&pokemenId](const HPokemen& pokemen) {
						return pokemen->GetId() == pokemenId;
					}
				);
				if (it != pokemens.end())
				{
					/* 向UI回传UPDATE信息 */
					char oldProp[BUFSIZ];
					sprintf(oldProp,
						"%d,%d,%s,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d\n",
						(*it)->GetId(), (int)(*it)->GetType(), (*it)->GetName().c_str(),
						(*it)->GetHpoints(), (*it)->GetAttack(), (*it)->GetDefense(), (*it)->GetAgility(),
						(*it)->GetInterval(), (*it)->GetCritical(), (*it)->GetHitratio(), (*it)->GetParryratio(),
						(*it)->GetCareer(), (*it)->GetExp(), (*it)->GetLevel()
					);
					msg.options.append(oldProp);

					/* 升级 */
					(*it)->Upgrade(raiseExp);

					sprintf(packet.data + std::strlen(packet.data),
						"%d,%d,%s,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d\n",
						(*it)->GetId(), (int)(*it)->GetType(), (*it)->GetName().c_str(),
						(*it)->GetHpoints(), (*it)->GetAttack(), (*it)->GetDefense(), (*it)->GetAgility(),
						(*it)->GetInterval(), (*it)->GetCritical(), (*it)->GetHitratio(), (*it)->GetParryratio(),
						(*it)->GetCareer(), (*it)->GetExp(), (*it)->GetLevel()
					);

					msg.options.append(packet.data);

					/* 向服务器回传UPDATE信息 */
					netDriver.SendPacket(packet);
				}

				return Message{
					MsgType::PVE_RESULT,
					ref new Platform::String(StringToWString(msg.options.c_str()).c_str())
				};
			}

			default:
				break;
			}
		}
		return { };
	}

	Message Core::ReadOnlineMessage()
	{
		if (!netDriver.IsConnected())
			return { };

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
				{
					::Pokemen::Property prop;
					int carrer;
					char name[BUFSIZ];
					sscanf(recvPacket.data,
						"%d\n%d\n%s\n%d\n%d\n%d\n%d\n%d\n%d\n%d\n%d\n%d\n%d\n%d\n",
						&prop.m_id, (int*)&prop.m_type, name,
						&prop.m_hpoints, &prop.m_attack, &prop.m_defense, &prop.m_agility,
						&prop.m_interval, &prop.m_critical, &prop.m_hitratio, &prop.m_parryratio,
						&carrer, &prop.m_exp, &prop.m_level
					);
					prop.m_name.assign(name);
					Pokemens::iterator it = std::find_if(pokemens.begin(),
						pokemens.end(), [&prop](const HPokemen& pokemen) {
						return pokemen->GetId() == prop.m_id;
					});
					if (it == pokemens.end())
					{
						pokemens.push_back(new ::Pokemen::Pokemen(
							prop, carrer
						));
					}
					else
					{
						(*it)->RenewProperty(prop, carrer);
					}
				}
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

	Property Core::GetPropertyAt(int pokemenId)
	{
		for (Pokemens::const_iterator it = pokemens.begin();
			it != pokemens.end(); ++it)
		{
			if ((*it)->GetId() == pokemenId)
			{
				return {
					(*it)->GetId(),
					(int)(*it)->GetType(),
					ref new Platform::String(
						StringToWString((*it)->GetName().c_str()).c_str()
					),
					(*it)->GetHpoints(),
					(*it)->GetAttack(),
					(*it)->GetDefense(),
					(*it)->GetAgility(),
					(*it)->GetInterval(),
					(*it)->GetCritical(),
					(*it)->GetHitratio(),
					(*it)->GetParryratio(),
					(*it)->GetCareer(),
					(*it)->GetExp(),
					(*it)->GetLevel()
				};
			}
		}
		return { };
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

	void Core::StartBattle()
	{
		stage.Start();
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