//! \file Behavior_TODO.cpp
//! \brief Implements the <code>fundamentals::Behavior</code> abstract class.
//! \author Jeremiah Blanchard with code from Cromwell D. Enage

#include "Behavior.h"
#include <queue>
#include <stack>

namespace ufl_cap4053 { namespace fundamentals {

	unsigned Behavior::createdCount = 0;
	unsigned Behavior::destroyedCount = 0;

	unsigned Behavior::getCreatedCount()
	{
		return createdCount;
	}

	unsigned Behavior::getDestroyedCount()
	{
		return destroyedCount;
	}

	Behavior::Behavior(char const* _description) : TreeNode<const char*>(_description) { }

}}  // namespace ufl_cap4053::fundamentals
