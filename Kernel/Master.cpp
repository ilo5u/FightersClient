#include "pch.h"

namespace Pokemen
{
	static const Strings namesOfMaster{ "Bulbasaur", "Ivysaur", "Venusaur", "Charmander" };

	Master::Career::Career(Career::Type type) :
		type(type)
	{
	}

	Master::Master(int level) :
		BasePlayer(
			PokemenType::MASTER, namesOfMaster[_Random(namesOfMaster.size())],
			BasicProperties::hitpoints + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::attack + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::defense + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::agility + _Random(CommonBasicValues::propertyOffset),
			0x0, 0x0, 0x0, 0x0
		),
		m_skill(static_cast<Skill::Type>(_Random(2))),
		m_career(Career::Type::Normal),
		m_angriedCnt(0x1)
	{
		this->m_property.m_interval
			= IntervalValueCalculator(
				CommonBasicValues::interval,
				this->m_property.m_agility,
				this->m_property.m_attack
			);
		this->m_property.m_critical
			= CriticalValueCalculator(
				CommonBasicValues::critical,
				this->m_property.m_agility
			);
		this->m_property.m_hitratio
			= HitratioValueCalculator(
				CommonBasicValues::hitratio,
				this->m_property.m_agility,
				this->m_property.m_attack
			);
		this->m_property.m_parryratio
			= ParryratioValueCalculator(
				CommonBasicValues::parryratio,
				this->m_property.m_agility,
				this->m_property.m_defense
			);

		while (this->m_property.m_level < std::min<Value>(level, CommonBasicValues::levelLimitation))
			Upgrade(CommonBasicValues::exp);

		if (level > 8)
			Promote(static_cast<Career::Type>(_Random(3)));
	}

	Master::Master(const Property& prop, Career::Type career) :
		BasePlayer(prop),
		m_skill(Skill::Type::RAGED),
		m_career(career),
		m_angriedCnt(0x1)
	{
	}

	Master::~Master()
	{
	}

	Master::Skill::Skill(Type primarySkill) :
		primarySkill(primarySkill)
	{
	}

	Master::Career::Type Master::GetCareer() const
	{
		return this->m_career.type;
	}

	void Master::SetCareer(Career::Type career)
	{
		this->m_career.type = career;
	}

	Master::Skill::Type Master::GetPrimarySkill() const
	{
		return this->m_skill.primarySkill;
	}

	/// <summary>
	/// 
	/// </summary>
	String Master::Attack(BasePlayer& opponent)
	{
		this->m_battleMessage[0] = 0x0;
		// 处理异常状态

		if (this->InState(State::WEAKEN))
		{
			if (this->m_stateRoundsCnt.weaken == 1)
			{
				this->m_property.m_attack -= this->m_effects.weaken.attack;
				this->m_property.m_defense -= this->m_effects.weaken.defense;
				this->SubState(State::WEAKEN);
			}
			else
			{
				--this->m_stateRoundsCnt.weaken;
			}
		}

		if (this->InState(State::SUNDERED))
		{
			if (this->m_stateRoundsCnt.sundered == 1)
			{
				this->m_property.m_attack -= this->m_effects.sundered.attack;
				this->SubState(State::SUNDERED);
			}
			else
			{
				--this->m_stateRoundsCnt.sundered;
			}
		}

		if (this->InState(State::SILENT))
		{
			if (this->m_stateRoundsCnt.silent == 1)
			{
				this->SubState(State::SILENT);
			}
			else
			{
				--this->m_stateRoundsCnt.silent;
			}
		}

		if (this->InState(State::SLOWED))
		{
			if (this->m_stateRoundsCnt.slowed == 1)
			{
				this->m_property.m_interval -= this->m_effects.slowed.interval;
				this->SubState(State::SLOWED);
			}
			else
			{
				--this->m_stateRoundsCnt.slowed;
			}
		}

		if (this->InState(State::DIZZYING))
		{
			if (this->m_stateRoundsCnt.dizzying == 1)
			{
				this->SubState(State::DIZZYING);
			}
			else
			{
				--this->m_stateRoundsCnt.dizzying;
			}
			return m_battleMessage;
		}

		if (this->InState(State::DEAD))
		{
			return m_battleMessage;
		}

		if (_Hit_Target(this->m_property.m_hitratio, opponent.GetParryratio()))
		{	// 命中目标
			Value damage = this->m_property.m_attack;

			if (_Hit_Target(this->m_property.m_critical, opponent.GetCritical()))
			{ // 暴击
				damage = static_cast<Value>((double)damage * 1.5);
			}

			this->_InitSkill_();
			/* 修正技能效果 */
			int selfHealingChance = this->m_skill.selfHealingChance;
			int selfHealingIndex = this->m_skill.selfHealingIndex;
			int weakenIndex = this->m_skill.weakenIndex;
			switch (this->m_career.type)
			{
			case Career::Type::GreatMasterOfLight:
				selfHealingChance += ConvertValueByPercent(selfHealingChance, Career::Lighter::selfHealingChanceIncIndex);
				selfHealingIndex += ConvertValueByPercent(selfHealingIndex, Career::Lighter::healingIncIndex);
				weakenIndex += ConvertValueByPercent(weakenIndex, Career::Lighter::weakenDecIndex);
				break;

			case Career::Type::GreatMasterOfDark:
				selfHealingChance += ConvertValueByPercent(selfHealingChance, Career::Darker::selfHealingChanceDecIndex);
				weakenIndex += ConvertValueByPercent(weakenIndex, Career::Darker::weakenIncIndex);
				break;

			default:
				break;
			}

			if (!this->InState(State::SILENT) && this->InState(State::ANGRIED))
			{
				// 重设怒气值
				this->m_anger = 0;
				this->SubState(State::ANGRIED);

				if (this->m_property.m_hpoints < (this->m_angriedCnt * this->m_angriedCnt * this->m_angriedCnt))
				{
					// 治愈系技能使用过度 强制使用攻击增益BUFF
					int inc = 0;
					switch (this->m_career.type)
					{
					case Career::Type::Normal:
						inc = static_cast<Value>((double)this->m_property.m_attack / 15.0);
						break;

					case Career::Type::GreatMasterOfLight:
						inc = static_cast<Value>((double)this->m_property.m_attack / 5.0);
						break;

					case Career::Type::GreatMasterOfDark:
						inc = static_cast<Value>((double)this->m_property.m_attack / 10.0);
						break;

					default:
						break;
					}
					sprintf(m_battleMessage + std::strlen(m_battleMessage),
						"致命打击，增加%d点攻击力。", inc);
					this->m_property.m_attack += inc;
				}
				else
				{
					// 治愈系技能
					sprintf(m_battleMessage + std::strlen(m_battleMessage),
						"死者苏生，恢复%d点生命值。",
						static_cast<Value>((double)this->m_property.m_hpoints / (double)this->m_angriedCnt));
					this->m_property.m_hpoints = std::min<Value>(
						this->m_hpointsLimitation,
						this->m_property.m_hpoints + 
						(this->m_career.type == Career::Type::GreatMasterOfLight 
							? 2 : 1) * static_cast<Value>((double)this->m_property.m_hpoints / (double)this->m_angriedCnt)
						);
					switch (this->m_career.type)
					{
					case Career::Type::Normal:
						this->m_angriedCnt += 4;
						break;

					case Career::Type::GreatMasterOfLight:
						this->m_angriedCnt += 2;
						break;

					case Career::Type::GreatMasterOfDark:
						this->m_angriedCnt += 5;
						break;

					default:
						break;
					}
				}

				if (!this->InState(State::WEAKEN))
				{
					// 狂暴后进入虚弱状态
					sprintf(m_battleMessage + std::strlen(m_battleMessage), "虚弱。");

					this->m_effects.weaken.attack = ConvertValueByPercent(this->m_property.m_attack, weakenIndex);
					this->m_effects.weaken.defense = ConvertValueByPercent(this->m_property.m_defense, weakenIndex);
					this->m_property.m_attack += this->m_effects.weaken.attack;
					this->m_property.m_defense += this->m_effects.weaken.defense;

					this->SetWeakenRounds(CommonBasicValues::weakenRounds);
					this->AddState(State::WEAKEN);
				}
			}
			else if (!this->InState(State::SILENT))
			{
				switch (this->m_skill.primarySkill)
				{
				case Skill::Type::RAGED:
				{
					if (_Hit_Target(this->m_skill.ragedChance, 0))
					{
						sprintf(m_battleMessage + std::strlen(m_battleMessage),
							"愤怒。");

						this->AddState(State::RAGED);
					}
					else if (_Hit_Target(selfHealingChance, 5))
					{
						sprintf(m_battleMessage + std::strlen(m_battleMessage),
							"自愈，回复%d点生命值。",
							HealingHpointsCalculator(this->m_property.m_hpoints, selfHealingIndex));

						this->m_skill.selfHealingChance = std::max<Value>((Value)10, this->m_skill.selfHealingChance - 1);
						this->m_skill.ragedChance = std::min<Value>((Value)30, this->m_skill.ragedChance + 1);

						this->m_property.m_hpoints = std::min<Value>(
							this->m_property.m_hpoints + HealingHpointsCalculator(this->m_property.m_hpoints, selfHealingIndex),
							this->m_hpointsLimitation
							);
					}
				}
				break;

				case Skill::Type::SELF_HEALING:
				{
					if (_Hit_Target(selfHealingChance, 0))
					{
						sprintf(m_battleMessage + std::strlen(m_battleMessage),
							"自愈，回复%d点生命值。",
							HealingHpointsCalculator(this->m_property.m_hpoints, selfHealingIndex));

						this->m_skill.selfHealingChance = std::max<Value>((Value)15, this->m_skill.selfHealingChance - 1);
						this->m_skill.ragedChance = std::min<Value>((Value)20, this->m_skill.ragedChance + 1);

						this->m_property.m_hpoints = std::min<Value>(
							this->m_property.m_hpoints + HealingHpointsCalculator(this->m_property.m_hpoints, selfHealingIndex),
							this->m_hpointsLimitation
							);
					}
					else if (_Hit_Target(this->m_skill.ragedChance, 5))
					{
						sprintf(m_battleMessage + std::strlen(m_battleMessage),
							"愤怒。");

						this->AddState(State::RAGED);
					}
				}
				break;

				default:
					break;
				}
			}
			// 攻击敌方小精灵
			sprintf(m_battleMessage + std::strlen(m_battleMessage),
				"造成%d点伤害。",
				AttackDamageCalculator(damage, opponent.GetDefense()));
			Value rebounce = opponent.IsAttacked(AttackDamageCalculator(damage, opponent.GetDefense()));
			if (rebounce > 0)
			{	// 对方开启反甲
				sprintf(m_battleMessage + std::strlen(m_battleMessage),
					"受到%d点反伤。", rebounce);
				this->m_property.m_hpoints -= rebounce;
			}

			if (this->m_property.m_hpoints <= 0)
			{
				sprintf(m_battleMessage + std::strlen(m_battleMessage),
					"小精灵死亡。");
				this->m_property.m_hpoints = 0;
				this->m_state = State::DEAD;
			}
			else if (this->m_career.type == Career::Type::GreatMasterOfDark)
			{
				this->m_anger = std::min<Value>(this->m_anger + Career::Darker::angerExtra, CommonBasicValues::angerLimitation);
				if (this->m_anger == CommonBasicValues::angerLimitation)
				{
					this->m_anger = 0;
					this->AddState(State::ANGRIED);
				}
			}
		}
		else
		{
			sprintf(m_battleMessage + std::strlen(m_battleMessage), "未命中。");
		}

		return m_battleMessage;
	}

	/// <summary>
	/// 
	/// </summary>
	Value Master::IsAttacked(Value damage)
	{
		if (damage >= this->m_property.m_hpoints)
		{
			this->m_property.m_hpoints = 0;
			this->m_state = State::DEAD;
		}
		else
		{
			this->m_property.m_hpoints -= damage;
			if (this->InState(State::RAGED))
			{
				this->m_anger = std::min<Value>(
					CommonBasicValues::angerLimitation, this->m_anger + CommonBasicValues::angerInc * (Value)2
					);
				this->SubState(State::RAGED);
			}
			else
			{
				this->m_anger = std::min<Value>(CommonBasicValues::angerLimitation, this->m_anger + 2 * CommonBasicValues::angerInc);
			}

			if (this->m_anger == CommonBasicValues::angerLimitation)
				this->AddState(State::ANGRIED);

			if (this->InState(State::BLEED))
			{
				this->m_property.m_hpoints -= BloodingDamageCalculator(CommonBasicValues::bleedDamage, this->m_property.m_defense);
				sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
					"出血造成%d点伤害。",
					BloodingDamageCalculator(CommonBasicValues::bleedDamage, this->m_property.m_defense));
				if (this->m_property.m_hpoints <= 0)
				{
					this->m_property.m_hpoints = 0;
					this->m_state = State::DEAD;
				}

				if (this->m_stateRoundsCnt.bleed == 1)
					this->SubState(State::BLEED);
			}
		}
		return 0;
	}

	bool Master::SetPrimarySkill(Skill::Type primarySkill)
	{
		this->m_skill.primarySkill = primarySkill;
		return false;
	}

	bool Master::Promote(Career::Type career)
	{
		if (this->m_career.type == Career::Type::Normal)
		{
			this->m_career.type = career;
			int bonus = (this->m_property.m_level - 8) * 10 + 100;
			switch (this->m_career.type)
			{
			case Career::Type::GreatMasterOfLight:
			{
				this->m_property.m_attack += ConvertValueByPercent(this->m_property.m_attack, Career::Lighter::damageDecIndex);
				this->m_property.m_hpoints += ConvertValueByPercent(ConvertValueByPercent(this->m_property.m_hpoints, Career::Lighter::hpointsIncIndex), bonus);
			}
			break;

			case Career::Type::GreatMasterOfDark:
			{
				this->m_property.m_defense += ConvertValueByPercent(this->m_property.m_defense, Career::Darker::defenseDecIndex);
				this->m_property.m_attack += ConvertValueByPercent(ConvertValueByPercent(this->m_property.m_attack, Career::Darker::damageIncIndex), bonus);
			}
			break;

			default:
				break;
			}
		}
		return false;
	}

	void Master::_LevelUpPropertiesDistributor_()
	{
		Value attackInc = 0;
		Value defenseInc = 0;
		Value agilityInc = 0;

		switch (this->m_career.type)
		{
		case Career::Type::Normal:
		{
			attackInc
				= _Random(CommonBasicValues::levelupPropertiesInc / 2);
			defenseInc 
				= _Random(CommonBasicValues::levelupPropertiesInc / 2);
			agilityInc
				= CommonBasicValues::levelupPropertiesInc - attackInc - defenseInc;

			this->m_property.m_hpoints
				+= static_cast<Value>((double)this->m_property.m_hpoints / 20.0) + CommonBasicValues::hpointsInc;
		}
		break;

		case Career::Type::GreatMasterOfLight:
		{
			defenseInc
				= _Random(static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 2), CommonBasicValues::levelupPropertiesInc);
			attackInc
				= _Random(CommonBasicValues::levelupPropertiesInc - defenseInc);
			agilityInc
				= CommonBasicValues::levelupPropertiesInc - attackInc - defenseInc;

			this->m_property.m_hpoints 
				+= ConvertValueByPercent(static_cast<Value>((double)this->m_property.m_hpoints / 20.0), Career::Lighter::hpointsIncIndex) + CommonBasicValues::hpointsInc;
		}
		break;

		case Career::Type::GreatMasterOfDark:
		{
			attackInc
				= _Random(static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 2), CommonBasicValues::levelupPropertiesInc);
			defenseInc 
				= _Random(CommonBasicValues::levelupPropertiesInc - attackInc);
			agilityInc 
				= CommonBasicValues::levelupPropertiesInc - attackInc - defenseInc;
			attackInc
				+= ConvertValueByPercent(attackInc, Career::Darker::damageIncIndex);

			this->m_property.m_hpoints 
				+= static_cast<Value>((double)this->m_property.m_hpoints / 40.0) + CommonBasicValues::hpointsInc;
		}
		break;

		default:
			break;
		}
		this->m_property.m_attack += attackInc;
		this->m_property.m_defense += defenseInc;
		this->m_property.m_agility += agilityInc;
	}

	void Master::_InitSkill_()
	{
		this->m_skill.ragedChance = +10;
		this->m_skill.selfHealingChance = +30;
		this->m_skill.selfHealingIndex = +20;
		this->m_skill.weakenIndex = -20;
	}
}