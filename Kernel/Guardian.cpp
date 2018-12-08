#include "pch.h"

namespace Pokemen
{
	static const Strings namesOfGuardian{ "Caterpie", "Metapod", "Butterfree", "Weedle" };

	Guardian::Career::Career(Career::Type type) :
		type(type)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	Guardian::Guardian(int level) :
		BasePlayer(
			PokemenType::GUARDIAN, namesOfGuardian[_Random(namesOfGuardian.size())],
			BasicProperties::hitpoints + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::attack + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::defense + _Random(CommonBasicValues::propertyOffset),
			BasicProperties::agility + _Random(CommonBasicValues::propertyOffset),
			0x0, 0x0, 0x0, 0x0
		),
		m_skill(static_cast<Skill::Type>(_Random(2))),
		m_career(Career::Type::Normal)
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

	Guardian::Guardian(const Property& prop, Career::Type career) :
		BasePlayer(prop),
		m_skill(Skill::Type::REBOUND_DAMAGE),
		m_career(career)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	Guardian::~Guardian()
	{
	}

	Guardian::Skill::Skill(Type primarySkill) :
		primarySkill(primarySkill)
	{
	}

	Guardian::Career::Type Guardian::GetCareer() const
	{
		return this->m_career.type;
	}

	void Guardian::SetCareer(Career::Type career)
	{
		this->m_career.type = career;
	}

	Guardian::Skill::Type Guardian::GetPrimarySkill() const
	{
		return this->m_skill.primarySkill;
	}

	String Guardian::Attack(BasePlayer& opponent)
	{
		this->m_battleMessage[0] = 0x0;

		/* 状态判决 */
		if (this->InState(State::DEAD))
			return { };

		if (this->InState(State::ARMOR))
		{
			if (this->m_stateRoundsCnt.armor == 1)
			{
				this->m_property.m_defense -= this->m_effects.armor.defense;
				this->SubState(State::ARMOR);
			}
			else
			{
				--this->m_stateRoundsCnt.armor;
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

		if (this->InState(State::DIZZYING))
		{
			if (this->m_stateRoundsCnt.dizzying == 1)
			{
				this->SubState(State::DIZZYING);
				return { };
			}
			else
			{
				--this->m_stateRoundsCnt.dizzying;
			}
		}

		/* 攻击判决 */
		if (_Hit_Target(this->m_property.m_hitratio, opponent.GetParryratio()))
		{
			Value damage = this->m_property.m_attack;

			if (_Hit_Target(this->m_property.m_critical, opponent.GetCritical()))
			{ // 暴击
				damage = static_cast<Value>((double)damage * 1.5);
			}

			this->_InitSkill_();
			/* 修正技能效果 */
			int sunkInSilenceChance = this->m_skill.sunkInSilenceChance;
			int reboundDamageChance = this->m_skill.reboundDamageChance;
			int silentRounds = CommonBasicValues::silentRounds;
			switch (this->m_career.type)
			{
			case Career::Type::Paladin:
				sunkInSilenceChance
					+= (sunkInSilenceChance, Career::Paladin::sunkInSilenceChanceIncIndex);
				reboundDamageChance
					+= (reboundDamageChance, Career::Paladin::reboundDamageChanceDecIndex);
				break;

			case Career::Type::Joker:
				sunkInSilenceChance
					+= (sunkInSilenceChance, Career::Joker::sunkInSilenceChanceIncIndex);
				reboundDamageChance
					+= (reboundDamageChance, Career::Joker::reboundDamageChanceIncIndex);
				silentRounds += Career::Joker::silentRoundsIncIndex;
				break;

			default:
				break;
			}

			/* 技能判决 */
			if (!this->InState(State::SILENT) && this->InState(State::ANGRIED))
			{
				sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
					"全副武装。");
				this->m_anger = 0;
				this->SubState(State::ANGRIED);
				/* 全副武装 */
				this->m_effects.armor.defense
					+= ConvertValueByPercent(this->m_property.m_defense, this->m_skill.defenseIndex);
				this->m_stateRoundsCnt.armor = BasicProperties::armorRounds;
				switch (this->m_career.type)
				{
				case Career::Type::Paladin:
					this->m_effects.armor.defense
						+= ConvertValueByPercent(this->m_effects.armor.defense, Career::Paladin::defenseIncIndex);
					++this->m_stateRoundsCnt.armor;
					break;

				case Career::Type::Joker:
					this->m_effects.armor.defense
						+= ConvertValueByPercent(this->m_effects.armor.defense, Career::Joker::defenseDecIndex);
					--this->m_stateRoundsCnt.armor;
					break;

				default:
					break;
				}
				this->m_property.m_defense += this->m_effects.armor.defense;
				this->AddState(State::ARMOR);
			}
			else if (!this->InState(State::SILENT))
			{
				switch (this->m_skill.primarySkill)
				{
				case Skill::Type::REBOUND_DAMAGE:
					/* 主修背刺 */
				{
					if (_Hit_Target(reboundDamageChance, 0))
					{
						/* 背刺 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"背刺。");
						this->AddState(State::REBOUND);
					}
					else if (_Hit_Target(sunkInSilenceChance, 5))
					{
						/* 沉默 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"沉默。");
						opponent.SetSilentRounds(silentRounds);
						opponent.AddState(State::SILENT);
					}
				}
				break;

				case Skill::Type::SUNK_IN_SILENCE:
				{
					/* 主修沉默 */
					if (_Hit_Target(sunkInSilenceChance, 0))
					{
						/* 沉默 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"沉默。");
						opponent.SetSilentRounds(silentRounds);
						opponent.AddState(State::SILENT);
					}
					else if (_Hit_Target(reboundDamageChance, 5))
					{
						/* 背刺 */
						sprintf(this->m_battleMessage + std::strlen(this->m_battleMessage),
							"背刺。");
						this->AddState(State::REBOUND);
					}
				}
				break;

				default:
					break;
				}
			}

			// 攻击敌方小精灵
			/* 伤害判决 */
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
		}
		else
		{
			sprintf(m_battleMessage + std::strlen(m_battleMessage), "未命中。");
		}

		return m_battleMessage;
	}

	Value Guardian::IsAttacked(Value damage)
	{
		Value back = 0;
		if (damage >= this->m_property.m_hpoints)
		{
			this->m_property.m_hpoints = 0;
			this->m_state = State::DEAD;
		}
		else
		{
			this->m_property.m_hpoints -= damage;
			this->m_anger = std::min<Value>(
				CommonBasicValues::angerLimitation,
				this->m_anger + CommonBasicValues::angerInc + _Random(CommonBasicValues::angerInc)
				);

			if (this->m_anger == CommonBasicValues::angerLimitation)
				this->AddState(State::ANGRIED);

			/* 出血 */
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

			/* 背刺 */
			if (this->InState(State::REBOUND))
			{
				back = ConvertValueByPercent(damage, this->m_skill.reboundDamageIndex);
				switch (this->m_career.type)
				{
				case Career::Type::Paladin:
					back += ConvertValueByPercent(back, Career::Paladin::reboundDamageIncIndex);
					break;

				case Career::Type::Joker:
					break;

				default:
					break;
				}
				this->SubState(State::REBOUND);
			}
		}
		return back;
	}

	bool Guardian::SetPrimarySkill(Skill::Type primarySkill)
	{
		this->m_skill.primarySkill = primarySkill;
		return false;
	}

	bool Guardian::Promote(Career::Type career)
	{
		if (this->m_career.type == Career::Type::Normal)
		{
			this->m_career.type = career;
			switch (this->m_career.type)
			{
			case Career::Type::Paladin:
			{
				this->m_property.m_defense
					+= ConvertValueByPercent(this->m_property.m_defense, Career::Paladin::defenseIncIndex);
				this->m_property.m_attack
					+= ConvertValueByPercent(this->m_property.m_attack, Career::Paladin::damageDecIndex);
				this->m_property.m_agility
					+= (this->m_property.m_agility, Career::Paladin::agilityDecIndex);
			}
			break;

			case Career::Type::Joker:
			{
				this->m_property.m_defense
					+= (this->m_property.m_defense, Career::Joker::defenseDecIndex);
			}
			break;

			default:
				break;
			}
		}
		return false;
	}

	void Guardian::_LevelUpPropertiesDistributor_()
	{
		Value attackInc = 0;
		Value defenseInc = 0;
		Value agilityInc = 0;

		switch (this->m_career.type)
		{
		case Career::Type::Normal:
		{
			defenseInc 
				= _Random(CommonBasicValues::levelupPropertiesInc / 2);
			attackInc 
				= _Random(CommonBasicValues::levelupPropertiesInc / 2);
			agilityInc 
				= CommonBasicValues::levelupPropertiesInc - defenseInc - attackInc;

			this->m_property.m_hpoints
				+= _Random(static_cast<Value>((double)this->m_property.m_hpoints / 15.0)) + CommonBasicValues::hpointsInc;
		}
		break;

		case Career::Type::Paladin:
		{
			defenseInc 
				= _Random(static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 2), CommonBasicValues::levelupPropertiesInc);
			attackInc 
				= _Random(CommonBasicValues::levelupPropertiesInc - defenseInc);
			agilityInc 
				= CommonBasicValues::levelupPropertiesInc - defenseInc - agilityInc;
			defenseInc 
				+= ConvertValueByPercent(defenseInc, Career::Paladin::defenseIncIndex);

			this->m_property.m_hpoints += _Random(static_cast<Value>((double)this->m_property.m_hpoints / 10.0)) + CommonBasicValues::hpointsInc;
		}
		break;

		case Career::Type::Joker:
		{
			agilityInc = _Random(static_cast<Value>(CommonBasicValues::levelupPropertiesInc / 2), CommonBasicValues::levelupPropertiesInc);
			attackInc = _Random(CommonBasicValues::levelupPropertiesInc - agilityInc);
			defenseInc = CommonBasicValues::levelupPropertiesInc - agilityInc - attackInc;
			defenseInc -= ConvertValueByPercent(defenseInc, Career::Joker::defenseDecIndex);

			this->m_property.m_hpoints += _Random(static_cast<Value>((double)this->m_property.m_hpoints / 20.0)) + CommonBasicValues::hpointsInc;
		}
		break;

		default:
			break;
		}
		this->m_property.m_attack += attackInc;
		this->m_property.m_defense += defenseInc;
		this->m_property.m_agility += agilityInc;
	}

	void Guardian::_InitSkill_()
	{
		this->m_skill.sunkInSilenceChance = +20;
		this->m_skill.reboundDamageChance = +40;
		this->m_skill.reboundDamageIndex = +50;
		this->m_skill.defenseIndex = +100;
	}

}
