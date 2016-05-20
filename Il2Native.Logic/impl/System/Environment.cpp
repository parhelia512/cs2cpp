#include "CoreLib.h"

// Method : System.Environment.GetProcessorCount()
int32_t CoreLib::System::Environment::GetProcessorCount()
{
	return std::thread::hardware_concurrency();
}

// Method : System.Environment.TickCount.get
int32_t CoreLib::System::Environment::get_TickCount()
{
	std::chrono::steady_clock::time_point t0;
	auto now = std::chrono::steady_clock::now() - t0;
	return now.count() / 100;
}

// Method : System.Environment._Exit(int)
void CoreLib::System::Environment::_Exit(int32_t exitCode)
{
	std::exit(exitCode);
}
