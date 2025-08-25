//! \file Agent.h
//! \brief Defines the <code>fundamentals::Agent</code> class.
//! \author Jeremiah Blanchard
#pragma once

#include "../BehaviorTree/Behavior.h"
#include "World.h"
#include <vector>
#include <string>

using namespace std;

namespace ufl_cap4053 { namespace fundamentals {

	struct Knowledge
	{
		// World-state information is tracked using an enumeration and flags for each location.
		enum locationState { CLEAR = 0, DEFINITE_WUMPUS, DEFINITE_PIT,
		                     POSSIBLE_WUMPUS, POSSIBLE_PIT, POSSIBLE_W_P, UNKNOWN = -1 };

		vector<vector<char>> stimuli; // Stimuli perceived by the agent
		vector<vector<char>> modelWorld; // Perceptions agent has had of world and information deduced

		unsigned x, y; // Location of agent in world currently
		
		unsigned wumpusX, wumpusY; // Location of the Wumpus (init to -1, -1)
		bool safeUnexploredLocationPresent; // Determines if there are safe, unexplored locations around this spot.
		bool hasArrow; // Whether or not our agent has the arrow
		bool hasGold; // Whether or not our agent has the gold

		void init(unsigned _x, unsigned _y, unsigned width, unsigned height);
		void shutdown();

		const char* getStateAsString(int _x, int _y);
		const char* getStimuliAsString(int _x, int _y);
	};

	//! \brief The Agent class for this project
	class Agent
	{
	public:
		Agent(World& _world, Behavior& _behavior, void (*_behaviorLog)(Behavior const*));
		Knowledge& getKnowledge();

		void enter(unsigned _x, unsigned _y);
		void update();
		void exit();

		// Agent actions
		bool pickUpGold();
		bool move(Direction direction);
		bool shoot(Direction direction);

	private:
		void perceive();

		World& world; // The outside world.
		Behavior& behavior; // Agent behavior
		Knowledge knowledge; // Knowledge the agent has about the world.
		void (*behaviorLog)(Behavior const*); // Behavior loggin function.
		// TODO: make behaviorLog a const pointer.
	};
}}  // namespace ufl_cap4053::fundamentals

