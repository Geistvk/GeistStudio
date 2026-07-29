#include <iostream>
#include <string>

int main()
{
    std::string input;

    while (std::getline(std::cin, input))
    {
        if (input == "exit")
            break;

        std::string result = "Antwort: " + input;

        std::cout << result << std::endl;
        std::cout.flush();
    }

    return 0;
}