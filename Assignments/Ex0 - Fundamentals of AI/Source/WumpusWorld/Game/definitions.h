//! \file Agent.h
//! \brief Defines the <code>fundamentals::Agent</code> class.
//! \author Jeremiah Blanchard
#pragma once

namespace ufl_cap4053 { namespace fundamentals {

	static const char NONE = 0x00000000,
						STENCH = 0x00000001,
						BREEZE = 0x00000002,
						GOLD = 0x00000004,
						UNEXPLORED = 0x00000008,
						PIT = 0x00000010,
						WUMPUS = 0x00000020,
						START = 0x00000040;

	enum Direction { UP, DOWN, LEFT, RIGHT };

}}  // namespace ufl_cap4053::fundamentals
