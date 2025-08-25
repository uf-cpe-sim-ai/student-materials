#include "../platform.h" // This file will make exporting DLL symbols simpler for students.

namespace ufl_cap4053
{
	namespace searches
	{
		class PathSearch
		{
		// CLASS DECLARATION GOES HERE
			public:
				DLLEXPORT PathSearch(); // EX: DLLEXPORT required for public methods - see platform.h
		};
	}
}  // close namespace ufl_cap4053::searches
