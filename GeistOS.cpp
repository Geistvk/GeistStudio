#include <iostream>
#include <string>
#include <vector>
#include <map>
#include <functional>
#include <unordered_map>
#include <windows.h>

#include <algorithm>
#include <cctype>
#include <memory>

#include <wininet.h>

#include <stdexcept>
#include <cctype>

#include <io.h>
#include <fcntl.h>

#include <dxgi.h>
#include <cstdlib>

//#include <imgui.h>
//#include <imgui_impl_win32.h>
//#include <imgui_impl_dx11.h>

#include <algorithm>
#include <richedit.h>
#include <regex>
#include <map>

#include <stdio.h>

#include "lib.cpp"
#include "rotatingGhost.cpp"
#include "UIElements/Window.h"
#include "UIElements/Window.cpp"

#include <functional>

#include <thread>
#include <chrono>
#include <cstdlib>
#include <regex>
#include <random>

#include <unknwn.h> // IUnknown
#include <dxgi.h>
#include <iomanip>

#include <fstream>
#include <sstream>
#include <vector>
#include <deque>
#include <string>
#include <chrono>
#include <thread>

#include <ctime>

#include <cstdio>
#include <memory>
#include <atomic>
#include <array>

#include <cstring>
#include <unistd.h>

#include "VariadicTable.h"

#ifdef _WIN32
    #include <windows.h>
#else
    #include <sys/stat.h>
    #include <sys/types.h>
#endif

#ifdef _WIN32
    #include <windows.h>
#else
    #include <unistd.h>   // für rmdir
    #include <limits.h>   // PATH_MAX
    #include <unistd.h>   // readlink
#endif

#include <filesystem>
#include <sstream>
#include <fstream>

#include <cstdio>
#include <memory>

#if defined(_WIN32)
#include <conio.h>
#else
#include <termios.h>
#include <unistd.h>
#endif

// ==========================
// User-Struktur
// ==========================
struct User {
    int userId;
    std::string preName;
    std::string lastName;
    std::string name;
    std::string password;
    std::string created;
    std::string rank;
    std::string userRights;
};

// ==========================
// Passwort-Eingabe unsichtbar
// ==========================
std::string getHiddenInput() {
    std::string input;

#if defined(_WIN32)
    char ch;
    while ((ch = _getch()) != '\r') { // Enter
        if (ch == '\b') { // Backspace
            if (!input.empty()) {
                input.pop_back();
                std::cout << "\b \b";
            }
        } else {
            input.push_back(ch);
            std::cout << '*'; // optional: Sternchen anzeigen
        }
    }
    std::cout << std::endl;
#else
    struct termios oldt, newt;
    tcgetattr(STDIN_FILENO, &oldt);
    newt = oldt;
    newt.c_lflag &= ~ECHO;
    tcsetattr(STDIN_FILENO, TCSANOW, &newt);
    std::getline(std::cin, input);
    tcsetattr(STDIN_FILENO, TCSANOW, &oldt);
#endif

    return input;
}

// ==========================
// Hilfsfunktion: String splitten
// ==========================
std::vector<std::string> split(const std::string& s, char delimiter) {
    std::vector<std::string> tokens;
    std::string token;
    for (char c : s) {
        if (c == delimiter) {
            if (!token.empty()) {
                tokens.push_back(token);
                token.clear();
            }
        } else {
            token.push_back(c);
        }
    }
    if (!token.empty()) tokens.push_back(token);
    return tokens;
}

std::string currentDictonary = "/";

// ==========================
// Globale aktuelle Farbe (Standard: Weiß)
// ==========================
std::string currentColor = "\033[0;37m"; 
std::string secColor = "\033[1;32m";


void printScreenOld(std::string type) {
    //font: Kban

    if (type == "Welcome") {
        std::cout << "\n";
        std::cout << currentColor + "\033[1;34m  ..|'''.|           ||           .       ..|''||    .|'''.|   \033[0m \n";
        std::cout << currentColor + "\033[1;34m .|'     '    ....  ...   ....  .||.     .|'    ||   ||..  '   \033[0m \n";
        std::cout << currentColor + "\033[1;34m ||    .... .|...||  ||  ||. '   ||      ||      ||   ''|||.   \033[0m \n";
        std::cout << currentColor + "\033[1;34m '|.    ||  ||       ||  . '|..  ||      '|.     || .     '||  \033[0m \n";
        std::cout << currentColor + "\033[1;34m  ''|...'|   '|...' .||. |'..|'  '|.'     ''|...|'  |'....|'   \033[0m \n\n"; 
        std::cout << "\n";
    } else if (type == "Login") {
        std::cout << "\n";
        std::cout << currentColor + "\033[1;34m '||'                       ||            \033[0m\n";
        std::cout << currentColor + "\033[1;34m  ||         ...     ... . ...  .. ...    \033[0m\n";
        std::cout << currentColor + "\033[1;34m  ||       .|  '|.  || ||   ||   ||  ||   \033[0m\n";
        std::cout << currentColor + "\033[1;34m  ||       ||   ||   |''    ||   ||  ||   \033[0m\n";
        std::cout << currentColor + "\033[1;34m .||.....|  '|..|'  '||||. .||. .||. ||.  \033[0m\n";
        std::cout << currentColor + "\033[1;34m                   .|....'                \033[0m\n\n";
        std::cout << "\n";                         
    } else if (type == "Help") {
        std::cout << "\n";
        std::cout << currentColor + "\033[1;34m '||'  '||'         '||           \033[1;34m\n";
        std::cout << currentColor + "\033[1;34m  ||    ||    ....   ||  ... ...  \033[1;34m\n";
        std::cout << currentColor + "\033[1;34m  ||''''||  .|...||  ||   ||'  || \033[1;34m\n";
        std::cout << currentColor + "\033[1;34m  ||    ||  ||       ||   ||    | \033[1;34m\n";
        std::cout << currentColor + "\033[1;34m .||.  .||.  '|...' .||.  ||...'  \033[1;34m\n";
        std::cout << currentColor + "\033[1;34m                          ||      \033[1;34m\n";
        std::cout << currentColor + "\033[1;34m                          ''''    \033[1;34m\n";
        std::cout << "\n";
    }
}

void printScreen(const std::string& text) {
    if (text.empty()) return;

    std::string upper;
    for (char c : text)
        upper += std::toupper(static_cast<unsigned char>(c));

    std::cout << "\n";

    constexpr int FONT_HEIGHT = 5;

    for (int row = 0; row < FONT_HEIGHT; ++row) {
        for (char c : upper)
        {
            auto it = FONT.find(c);
            if (it != FONT.end())
                std::cout << currentColor << "\033[1;34m"
                          << it->second[row] << " \033[0m";
            else
                std::cout << "      "; // unbekanntes Zeichen → leer
        }

        std::cout << "\n";
    }

    std::cout << std::endl; // EINMAL newline → fertig
}

static inline std::string ltrim(const std::string& s) {
    size_t start = 0;
    while (start < s.size() && std::isspace(static_cast<unsigned char>(s[start])))
        start++;
    return s.substr(start);
}

static inline std::string rtrim(const std::string& s) {
    if (s.empty()) return s;

    size_t end = s.size() - 1;
    while (end > 0 && std::isspace(static_cast<unsigned char>(s[end])))
        end--;

    return s.substr(0, end + 1);
}

static inline std::string trim(const std::string& s) {
    return rtrim(ltrim(s));
}


// Hilfsfunktion: Mapping von CMD-Farbcode zu ANSI
std::string getAnsiColor(char code) {
    switch (toupper(code)) {
        case '0': return "\033[0;30m"; // Schwarz
        case '1': return "\033[0;34m"; // Dunkelblau
        case '2': return "\033[0;32m"; // Dunkelgrün
        case '3': return "\033[0;36m"; // Dunkelcyan
        case '4': return "\033[0;31m"; // Dunkelrot
        case '5': return "\033[0;35m"; // Dunkellila
        case '6': return "\033[0;33m"; // Dunkelgelb
        case '7': return "\033[0;37m"; // Hellgrau
        case '8': return "\033[1;30m"; // Dunkelgrau
        case '9': return "\033[1;34m"; // Hellblau
        case 'A': return "\033[1;32m"; // Hellgrün
        case 'B': return "\033[1;36m"; // Hellcyan
        case 'C': return "\033[1;31m"; // Hellrot
        case 'D': return "\033[1;35m"; // Helllila
        case 'E': return "\033[1;33m"; // Hellgelb
        case 'F': return "\033[1;37m"; // Weiß
        default:  return "\033[0;37m"; // Fallback Weiß
    }
}

std::atomic<bool> animationRunning(false);
std::atomic<bool> stopAnimation(false);
std::atomic<std::chrono::steady_clock::time_point> lastInput;

// -------------------------------------------------
// GHOST ASCII FRAMES
// -------------------------------------------------
std::vector<std::string> frames = {
    R"(
       .-.
     .'   `.
     :      :
     |      |
     :      :
     `.    .'
       `--'
    )",
    R"(
       .-.
     .'o  `.
     :   o  :
     |  o   |
     :   o  :
     `.  o .'
       `--'
    )",
    R"(
       .-.
     .' *  `.
     :   *  :
     |  *   |
     :   *  :
     `.  * .'
       `--'
    )"
};

// -------------------------------------------------
// Helper: Clear screen using ANSI escape codes
// -------------------------------------------------
void clearScreen() {
    //std::cout << "\033[2J\033[H"; // clear screen + move cursor home
    system("cls");
}

bool anyKeyPressed()
{
    for (int key = 0x08; key <= 0xFE; ++key) {
        // LOW-ORDER BIT = Taste wurde gedrückt (nicht gehalten!)
        if (GetAsyncKeyState(key) & 0x0001)
        {
            return true;
        }
    }
    return false;
}



// -------------------------------------------------
// GHOST ANIMATION THREAD (läuft unabhängig)
// -------------------------------------------------
void animationThread()
{
    size_t idx = 0;
    (void)idx;
    stopAnimation = false;

    while (!stopAnimation) {
        animateGhost();
    }

    animationRunning = false;
}

// -------------------------------------------------
// IDLE MONITOR THREAD (überwacht Inaktivität)
// -------------------------------------------------
void idleMonitor()
{
    while (true) {
        auto now = std::chrono::steady_clock::now();
        long diff = std::chrono::duration_cast<std::chrono::seconds>(now - lastInput.load()).count();

        if (diff >= 30 && !animationRunning.load()) {
            animationRunning = true;
            std::thread(animationThread).detach();
            std::cout << currentColor + "\n\033[1;36mYou are now AFK! Press enter to resume\033[0m\n";
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(200));
    }
}

auto systemStart = std::chrono::steady_clock::now();







class DataBase {
public:
    struct Condition {
        std::string column;
        std::string op;
        std::string value;
    };
private:
    struct Table {
        std::vector<std::string> columns;
        std::vector<std::vector<std::string>> rows;
    };

    struct Database {
        std::string name;
        Table table;
    };

    std::vector<Database> db;

    const char COL_RESET   = '7';
    const char COL_ERROR   = 'C';
    const char COL_SUCCESS = '2';
    const char COL_WARN    = 'E';
    const char COL_TITLE   = '9';
    const char COL_NAME    = '3';
    const char COL_BORDER  = 'B';
    const char COL_VALUE   = 'F';
    const char COL_HEADER  = '6';

    std::string trim(const std::string& str) {
        size_t start = str.find_first_not_of(" \t\r\n");
        size_t end = str.find_last_not_of(" \t\r\n");

        if (start == std::string::npos) return "";
        return str.substr(start, end - start + 1);
    }

    void printLine(int width) {
        for (int i = 0; i < width; i++) std::cout << "-";
        std::cout << "\n";
    }

    int getColumnIndex(const Table& table, const std::string& col) {
        for (size_t i = 0; i < table.columns.size(); i++) {
            if (table.columns[i] == col) return i;
        }
        return -1;
    }

    bool matchWhere(const std::vector<std::string>& row, int colIndex, const std::string& value) {
        if (colIndex < 0 || colIndex >= (int)row.size()) return false;
        return row[colIndex] == value;
    }

    bool compareValues(const std::string& a, const std::string& op, const std::string& b) {
        try {
            int ai = std::stoi(a);
            int bi = std::stoi(b);

            if (op == "==")  return ai == bi;
            if (op == "!=") return ai != bi;
            if (op == ">")  return ai > bi;
            if (op == "<")  return ai < bi;
            if (op == ">=") return ai >= bi;
            if (op == "<=") return ai <= bi;
        } catch (...) {
            if (op == "==")  return a == b;
            if (op == "!=") return a != b;
            if (op == ">")  return a > b;
            if (op == "<")  return a < b;
            if (op == ">=") return a >= b;
            if (op == "<=") return a <= b;
        }

        return false;
    }

    bool validateConditions(
        const Table& table,
        const std::vector<Condition>& conditions
    ) {
        for (const auto& cond : conditions) {
            int idx = getColumnIndex(table, cond.column);

            if (idx == -1) {
                std::cout << getAnsiColor(COL_ERROR)
                        << "Error: Column not found -> "
                        << getAnsiColor(COL_NAME) << cond.column
                        << getAnsiColor(COL_RESET) << "\n";
                return false;
            }
        }
        return true;
    }

    bool matchCondition(
        const Table& table,
        const std::vector<std::string>& row,
        const Condition& cond
    ) {
        int index = getColumnIndex(table, cond.column);
        if (index == -1) return false;

        return compareValues(row[index], cond.op, cond.value);
    }

    bool matchConditions(
        const Table& table,
        const std::vector<std::string>& row,
        const std::vector<Condition>& conditions,
        const std::vector<std::string>& logic // "and", "or"
    ) {
        if (conditions.empty()) return true;

        bool result = matchCondition(table, row, conditions[0]);

        for (size_t i = 1; i < conditions.size(); i++) {
            bool current = matchCondition(table, row, conditions[i]);

            if (logic[i - 1] == "and") {
                result = result && current;
            } else if (logic[i - 1] == "or") {
                result = result || current;
            }
        }

        return result;
    }

    Table filterTable(
        const Table& table,
        const std::vector<DataBase::Condition>& conditions,
        const std::vector<std::string>& logic
    ) {
        Table result;
        result.columns = table.columns;

        for (const auto& row : table.rows) {
            if (matchConditions(table, row, conditions, logic)) {
                result.rows.push_back(row);
            }
        }

        return result;
    }

    void printTable(const Table& table, const std::string& dbName) {
        std::cout << getAnsiColor(COL_TITLE)
                << "\n=== TABLE: "
                << getAnsiColor(COL_NAME) << dbName
                << getAnsiColor(COL_TITLE) << " ===\n"
                << getAnsiColor(COL_RESET);

        if (table.columns.empty()) {
            std::cout << getAnsiColor(COL_WARN)
                    << "(no columns)\n"
                    << getAnsiColor(COL_RESET);
            return;
        }

        std::vector<size_t> widths(table.columns.size());

        for (size_t i = 0; i < table.columns.size(); ++i) {
            widths[i] = table.columns[i].size();
        }

        for (const auto& row : table.rows) {
            for (size_t i = 0; i < row.size(); ++i) {
                widths[i] = std::max(widths[i], row[i].size());
            }
        }

        auto repeat = [](const std::string& str, size_t count) {
            std::string result;

            for (size_t i = 0; i < count; ++i) {
                result += str;
            }

            return result;
        };

        auto buildLine = [&](const std::string& left,
                            const std::string& middle,
                            const std::string& right)
        {
            std::string line = getAnsiColor(COL_BORDER);
            line += left;

            for (size_t i = 0; i < widths.size(); ++i) {
                line += repeat("─", widths[i] + 2);
                line += (i + 1 < widths.size()) ? middle : right;
            }

            line += getAnsiColor(COL_RESET);
            line += "\n";

            return line;
        };

        std::cout << buildLine("┌", "┬", "┐");

        std::cout << getAnsiColor(COL_BORDER) << "│" << getAnsiColor(COL_RESET);

        for (size_t i = 0; i < table.columns.size(); ++i) {
            std::cout << " "
                    << getAnsiColor(COL_HEADER)
                    << table.columns[i]
                    << getAnsiColor(COL_RESET)
                    << repeat(" ", widths[i] - table.columns[i].size())
                    << " "
                    << getAnsiColor(COL_BORDER)
                    << "│"
                    << getAnsiColor(COL_RESET);
        }

        std::cout << "\n";

        std::cout << buildLine("├", "┼", "┤");

        if (table.rows.empty()) {
            std::cout << getAnsiColor(COL_BORDER)
                    << "│ "
                    << getAnsiColor(COL_WARN)
                    << "(no data)"
                    << getAnsiColor(COL_RESET);

            size_t totalWidth = 0;
            for (size_t width : widths) {
                totalWidth += width + 3;
            }

            std::cout << repeat(" ", totalWidth - 11)
                    << getAnsiColor(COL_BORDER)
                    << "│"
                    << getAnsiColor(COL_RESET)
                    << "\n";

            std::cout << buildLine("└", "┴", "┘");
            std::cout << "\n";
            return;
        }

        for (const auto& row : table.rows) {

            std::cout << getAnsiColor(COL_BORDER)
                    << "│"
                    << getAnsiColor(COL_RESET);

            for (size_t i = 0; i < row.size(); ++i) {

                std::cout << " "
                        << getAnsiColor(COL_VALUE)
                        << row[i]
                        << getAnsiColor(COL_RESET)
                        << repeat(" ", widths[i] - row[i].size())
                        << " "
                        << getAnsiColor(COL_BORDER)
                        << "│"
                        << getAnsiColor(COL_RESET);
            }

            std::cout << "\n";
        }

        std::cout << buildLine("└", "┴", "┘");
        std::cout << "\n";
    }

    Database* findDB(const std::string& name) {
        for (auto& d : db) {
            if (d.name == name) {
                return &d;
            }
        }
        return nullptr;
    }

    bool confirmAction(std::string action, std::string name) {
        std::string confirm;
        std::cout << getAnsiColor(COL_WARN)
                << "Are you sure you want to " << action << " Database: "
                << getAnsiColor(COL_NAME)   << name
                << getAnsiColor(COL_WARN)   << "? (yes/no)"
                << getAnsiColor(COL_RESET)  << ": ";
        std::cin >> confirm;

        if (confirm != "yes") {
            std::cout << getAnsiColor(COL_SUCCESS)
                    << "[!] Operation cancelled\n"
                    << getAnsiColor(COL_RESET);
            return false;
        }
        return true;
    }




public:
    Condition parseCondition(const std::string& input) {
        std::vector<std::string> ops = {"<=", ">=", "!=", "==", "<", ">"};

        for (const auto& op : ops) {
            size_t pos = input.find(op);
            if (pos != std::string::npos) {
                return {
                    input.substr(0, pos),
                    op,
                    input.substr(pos + op.size())
                };
            }
        }

        return {"", "", ""};
    }

    void create(const std::string& name, const std::vector<std::string>& cols) {
        if (findDB(name)) {
            std::cout << getAnsiColor(COL_ERROR)
                    << "Error: DB already exists -> "
                    << getAnsiColor(COL_NAME) << name
                    << getAnsiColor(COL_RESET) << "\n";
            return;
        }

        if (cols.empty()) {
            std::cout << getAnsiColor(COL_ERROR)
                    << "Error: No columns provided\n"
                    << getAnsiColor(COL_RESET);
            return;
        }

        Database database;
        database.name = name;
        database.table.columns = cols;

        db.push_back(database);

        std::cout << getAnsiColor(COL_SUCCESS)
                << "\n[+] Database created\n"
                << getAnsiColor(COL_RESET);

        std::cout << " Name: "
                << getAnsiColor(COL_NAME) << name
                << getAnsiColor(COL_RESET) << "\n";

        std::cout << " Columns: ";

        for (const auto& c : cols) {
            std::cout << getAnsiColor(COL_HEADER)
                    << c << " "
                    << getAnsiColor(COL_RESET);
        }

        std::cout << "\n\n";
    }

    void insert(const std::string& name, const std::vector<std::string>& values) {
        Database* database = findDB(name);

        if (!database) {
            std::cout << getAnsiColor(COL_ERROR)
                    << "Error: DB not found -> "
                    << getAnsiColor(COL_NAME) << name
                    << getAnsiColor(COL_RESET) << "\n";
            return;
        }

        auto& table = database->table;

        if (values.size() != table.columns.size()) {
            std::cout << getAnsiColor(COL_ERROR)
                    << "Error: expected "
                    << table.columns.size()
                    << " values\n"
                    << getAnsiColor(COL_RESET);
            return;
        }

        table.rows.push_back(values);

        std::cout << getAnsiColor(COL_SUCCESS)
                << "[+] Row inserted into "
                << getAnsiColor(COL_NAME) << name
                << getAnsiColor(COL_RESET) << "\n";
    }

    void queryAdvanced(
        const std::string& name,
        const std::vector<Condition>& conditions,
        const std::vector<std::string>& logic,
        const std::string& orderCol = ""
    ) {
        Database* database = findDB(name);

        if (!database) {
            std::cout << getAnsiColor(COL_ERROR) << "DB not found\n" << getAnsiColor(COL_RESET);
            return;
        }

        auto& table = database->table;

        if (!validateConditions(table, conditions)) {
            return;
        }

        if (!orderCol.empty()) {
            int idx = getColumnIndex(table, orderCol);

            if (idx == -1) {
                std::cout << getAnsiColor(COL_ERROR)
                        << "Error: ORDER column not found -> "
                        << getAnsiColor(COL_NAME) << orderCol
                        << getAnsiColor(COL_RESET) << "\n";
                return;
            }
        }

        int orderIndex = -1;
        if (!orderCol.empty()) {
            orderIndex = getColumnIndex(table, orderCol);
        }

        std::vector<std::vector<std::string>> result;

        for (const auto& row : table.rows) {
            if (matchConditions(table, row, conditions, logic)) {
                result.push_back(row);
            }
        }

        if (orderIndex != -1) {
            std::sort(result.begin(), result.end(),
                [orderIndex](const auto& a, const auto& b) {
                    return a[orderIndex] < b[orderIndex];
                }
            );
        }

        auto& tablePrint = database->table;

        Table filtered = filterTable(tablePrint, conditions, logic);
        printTable(filtered, database->name);
    }

    void list() {
        std::cout << getAnsiColor(COL_TITLE)
                << "\n=== DATABASES ===\n"
                << getAnsiColor(COL_RESET);

        if (db.empty()) {
            std::cout << getAnsiColor(COL_WARN)
                    << "(none)\n"
                    << getAnsiColor(COL_RESET);
            return;
        }

        for (const auto& d : db) {
            std::cout << getAnsiColor(COL_NAME)
                    << "- " << d.name
                    << getAnsiColor(COL_RESET)
                    << " | cols: " << d.table.columns.size()
                    << " | rows: " << d.table.rows.size()
                    << "\n";
        }

        std::cout << "\n";
    }

    void drop(const std::string& name) {
        if (confirmAction("delete", name)) {
            for (auto it = db.begin(); it != db.end(); ++it) {
                if (it->name == name) {
                    db.erase(it);

                    std::cout << getAnsiColor(COL_SUCCESS)
                            << "[+] Deleted Database: "
                            << getAnsiColor(COL_NAME) << name
                            << getAnsiColor(COL_RESET) << "\n";
                    return;
                }
            }

            std::cout << getAnsiColor(COL_ERROR)
                    << "Error: Database not found\n"
                    << getAnsiColor(COL_RESET);
        }
    }

    void clear(const std::string& name) {
        if (confirmAction("clear", name)) {
            Database* database = findDB(name);

            if (!database) {
                std::cout << getAnsiColor(COL_ERROR)
                        << "Error: Database not found\n"
                        << getAnsiColor(COL_RESET);
                return;
            }

            database->table.rows.clear();

            std::cout << getAnsiColor(COL_SUCCESS)
                    << "[+] Cleared Database: "
                    << getAnsiColor(COL_NAME) << name
                    << getAnsiColor(COL_RESET) << "\n";
        }
    }
};





class SysLog {
public: 
    struct Permissions {
        bool Read;
        bool Write;
        bool Execute;
        bool Sudo;
    };
    struct Data {
        std::string name;
        int id;
        std::string perm;
        std::string octVal;
    };
    struct Command {
        Data data;
        std::function<std::string(const std::vector<std::string>&, const std::string&)> func;
        Permissions perm;
    };

    struct LogEntry {
        int id;
        std::string timestamp;
        std::string cmd;
        Command command;
        User* user;
    };

    std::vector<LogEntry> logs;

    std::string convertBoolToRights(std::vector<bool> bools) {
        std::string rights = "";

        for (size_t i = 0; i < bools.size(); i++) {
            if (bools[i]) rights += "1";
            else rights += "0";
        }

        return rights;
    }

    std::string convertRights(std::string userRightsTmp) {
        std::string userRights = "";
        std::string allRights = "rwxs";

        for (int i = 0; i < 4; i++) {
            if (userRightsTmp[i] == '1') {
                userRights += allRights[i];
            } else {
                userRights += '-';
            }
        }

        return userRights;
    }

    std::string calcOctalValue(std::string rightsTmp) {
        size_t octalVal = 0;

        for (size_t i = 0; i < rightsTmp.size(); i++) {
            if (rightsTmp[(rightsTmp.size() - 1) - i] == '1') 
                octalVal += pow(2, i);
        }

        return std::to_string(octalVal);
    }
private:
    int nextLogId = 1;

    std::string reset = "\033[0m";

    std::string colLogId         = getAnsiColor('3');
    std::string colTime          = getAnsiColor('8');
    std::string colUserInput     = getAnsiColor('2');
    std::string colCmdId         = getAnsiColor('1');
    std::string colCommandName   = getAnsiColor('9');
    std::string colUser          = getAnsiColor('B');
    std::string colRank          = getAnsiColor('5');
    std::string colRights        = getAnsiColor('6');
    std::string colPerm          = getAnsiColor('E');
    std::string colRequired      = getAnsiColor('A');
    std::string lineColor        = getAnsiColor('7');

    struct Column {
        std::string header;
        std::function<std::string(const LogEntry&)> value;
        std::string color;
        size_t width = 0;
    };

    std::vector<Column> columns;

    static std::string repeat(const std::string& str, size_t count) {
        std::string result;
        for (size_t i = 0; i < count; i++)
            result += str;
        return result;
    }

    static std::string padRight(const std::string& text, size_t width) {
        if (text.length() >= width)
            return text;
        return text + std::string(width - text.length(), ' ');
    }

    static std::string padLeft(const std::string& text, size_t width) {
        if (text.length() >= width)
            return text;
        return std::string(width - text.length(), ' ') + text;
    }

    const char* convertInt(int num) {
        static char buffer[6];
        sprintf(buffer, "%05d", num);
        return buffer;
    }

    static std::string getTimeStamp() {
        auto now = std::chrono::system_clock::now();
        std::time_t t = std::chrono::system_clock::to_time_t(now);

        std::tm tm = *std::localtime(&t);

        std::ostringstream oss;
        oss << std::put_time(&tm, "%d.%m.%Y %H:%M:%S");
        return oss.str();
    }

    void buildColumns() {
        columns = {
            { "LogId",          [&](const LogEntry& log){ return convertInt(log.id); },                     colLogId },
            { "Time",           [&](const LogEntry& log){ return log.timestamp; },                          colTime },
            { "User Input",     [&](const LogEntry& log){ return log.cmd; },                                colUserInput },
            { "CmdId",          [&](const LogEntry& log){ return convertInt(log.command.data.id); },        colCmdId },
            { "CommandName",    [&](const LogEntry& log){ return log.command.data.name; },                  colCommandName },
            { "User",           [&](const LogEntry& log){ return log.user->name; },                         colUser },
            { "Rank",           [&](const LogEntry& log){ return log.user->rank; },                         colRank },
            { "OctalValue",     [&](const LogEntry& log){ return calcOctalValue(log.user->userRights); },   colRights },
            { "Rights",         [&](const LogEntry& log){ return log.user->userRights; },                   colRights },
            { "Permissions",    [&](const LogEntry& log){ return convertRights(log.user->userRights); },    colPerm },
            { "RequiredOctVal", [&](const LogEntry& log){ return calcOctalValue(log.command.data.perm); },  colRequired },
            { "RequiredRights", [&](const LogEntry& log){ return log.command.data.perm; },                  colRequired },
            { "Cmd Permission", [&](const LogEntry& log){ return convertRights(log.command.data.perm); },   colRequired }
        };
    }

    void calcWidth() {
        size_t padding = 0;

        for (auto& col : columns) {
            col.width = col.header.length();

            for (const auto& log : logs) {
                col.width = std::max(col.width, col.value(log).length());
            }
            col.width += padding;
        }
    }

public:
    void addLog(const std::string& input, Command cmd, User* user) {
        logs.push_back({
            nextLogId++,
            getTimeStamp(),
            input,
            cmd,
            user
        });
    }

    void getCmdLog() {
        buildColumns();
        calcWidth();

        std::string out;

        out += lineColor + "┌";
        for (size_t i = 0; i < columns.size(); i++) {
            out += repeat("─", columns[i].width + 2);
            out += (i + 1 < columns.size()) ? "┬" : "┐";
        }
        out += reset + "\n";

        out += lineColor + "│ " + reset;

        for (size_t i = 0; i < columns.size(); i++) {
            auto& c = columns[i];

            out += c.color;
            out += padRight(c.header, c.width);
            out += reset;

            out += lineColor + " │ " + reset;
        }

        out += "\n";

        out += lineColor + "├";
        for (size_t i = 0; i < columns.size(); i++) {
            out += repeat("─", columns[i].width + 2);
            out += (i + 1 < columns.size()) ? "┼" : "┤";
        }
        out += reset + "\n";

        for (const auto& log : logs) {
            out += lineColor + "│ " + reset;

            for (size_t i = 0; i < columns.size(); i++) {
                auto& c = columns[i];

                std::string val = c.value(log);

                if (c.header == "ID" || c.header == "LogId")
                    val = padLeft(val, c.width);
                else
                    val = padRight(val, c.width);

                out += c.color + val + reset;
                out += lineColor + " │ " + reset;
            }

            out += "\n";
        }

        out += lineColor + "└";
        for (size_t i = 0; i < columns.size(); i++) {
            out += repeat("─", columns[i].width + 2);
            out += (i + 1 < columns.size()) ? "┴" : "┘";
        }
        out += reset + "\n";

        std::cout << out;
    }

    void clearCmdLog() {
        logs.clear();
        nextLogId = 1;
    }
};




DataBase db;
SysLog sysLog;























// ==========================
// Terminal-Klasse 
// ==========================
class Terminal {
private:
    struct Personal {
        std::string preName;
        std::string lastName;
        std::string password;
        std::string created;
    };
    struct userPerm {
        std::string rank;
        std::string userRights;
    };
    struct addUser {
        std::string userName;
        Personal personal;
        userPerm userPerm;
    };
    bool running = true;
    std::map<std::string, SysLog::Command> commands;

    std::string error = "none";

    int cmdId = 1;

    std::string standard   = "\033[0m";
    std::string textColor1 = getAnsiColor('1');
    std::string textColor2 = getAnsiColor('3');
    std::string errorColor = getAnsiColor('4');
    std::string accentColor= getAnsiColor('6');
    std::string sudoColor  = getAnsiColor('B');

    std::string btos(bool x) {
        if (x)
            return "True";
        return "False";
    }

    void initUsers(std::vector<addUser> addUsers) {
        int userIndex = 0;
        for (auto user : addUsers) {
            if (!noColor) {
                db.insert("users", {
                    std::to_string(userIndex),
                    user.personal.preName,
                    user.personal.lastName,
                    user.userName,
                    user.personal.password,
                    user.personal.created,
                    user.userPerm.rank,
                    sysLog.calcOctalValue(user.userPerm.userRights),
                    user.userPerm.userRights,
                    sysLog.convertRights(user.userPerm.userRights)
                    });
            }

            users[user.userName] = {
                userIndex,
                user.personal.preName, 
                user.personal.lastName, 
                user.userName, 
                user.personal.password, 
                user.personal.created, 
                user.userPerm.rank, 
                user.userPerm.userRights
            };
            userIndex++;
        }
    }

public:
    std::unordered_map<std::string, User> users;
    User* currentUser = nullptr;

    bool noColor = true;

    Terminal() {
        SetConsoleOutputCP(CP_UTF8);
        SetConsoleCP(CP_UTF8);

        if (!noColor)
            db.create("users", {"UserId", "PreName", "LastName", "UserName", "Password", "Created", "Rank", "OctalValue", "UserRights", "UserPermission"});

        initUsers({
            {"root",    {"Root", "Root", "root123", "2024"},        {"Owner", "1111"}},
            {"user",    {"User", "User", "user123", "2024"},        {"Member", "1010"}},
            {"Bot",     {"Bot", "Bot", "bot123", "2024"},           {"Bot", "0010"}},
            {"Admin",   {"Admin", "Admin", "admin123", "2024"},     {"Admin", "1111"}},
            {"Jonas",   {"Jonas", "Broschinski", "12345", "2024"},  {"Admin", "1111"}}
        });

        if (!noColor)
            db.create("commands", {"Name", "CmdId", "Permission", "OctVal", "ReqRead", "ReqWrite", "ReqExecute", "ReqSudo"});
    }

    void registerCommand(const std::string& name,
                         std::function<std::string(const std::vector<std::string>&, const std::string&)> func,
                         bool requiresRead = false,
                         bool requiresWrite = false,
                         bool requiresExecute = false,
                         bool requiresSudo = false) {
        std::string cmdPerm = sysLog.convertBoolToRights({
                                        requiresRead, 
                                        requiresWrite, 
                                        requiresExecute, 
                                        requiresSudo
                                    });
        commands[name] = {
            {
                name, 
                cmdId,
                cmdPerm,
                sysLog.calcOctalValue(cmdPerm)
            }, func, {
                requiresRead, 
                requiresWrite, 
                requiresExecute, 
                requiresSudo
            }
        };

        if (!noColor) {
            db.insert("commands", {
                name,
                std::to_string(cmdId),
                cmdPerm,
                sysLog.calcOctalValue(cmdPerm),
                btos(requiresRead),
                btos(requiresWrite),
                btos(requiresExecute),
                btos(requiresSudo)
                });
        }
        cmdId++;
    }

    boolean handleRights(std::string userRights, int pos) {
        if (userRights[pos] == '1') {
            return true;
        } else {
            return false;
        }
    }

    static std::string getTimeStamp() {
        auto now = std::chrono::system_clock::now();
        std::time_t t = std::chrono::system_clock::to_time_t(now);

        std::tm tm = *std::localtime(&t);

        std::ostringstream oss;
        oss << std::put_time(&tm, "%d.%m.%Y %H:%M:%S");
        return oss.str();
    }

    bool loginPrompt() {
        SetConsoleTitleA("Geist OS");

        std::string username, password;

        system("cls");
        printScreen("Login");

        if (error != "none") {
            std::cout << currentColor + error;
        }

        std::cout << currentColor + "\033[0;36m======\033[0m GeistOS Login \033[0;36m======\033[0m\n";
        std::cout << currentColor + "Admin Username:       root\n";
        std::cout << currentColor + "Admin User Password:  root123\n\n";

        std::cout << currentColor + "\033[1;32mUsername\033[0m: ";
        std::getline(std::cin, username);

        if (users.find(username) == users.end()) {
            error = errorColor + "Username incorrect.\n" + standard;
            return false;
        }

        std::cout << currentColor + "\033[1;32mPassword\033[0m: ";
        password = getHiddenInput();

        if (users[username].password == password) {
            #if defined(_WIN32)
                system("cls");
            #else
                system("clear");
            #endif

            currentUser = &users[username];
            printScreen("Geist OS");

            std::cout << currentColor + "Welcome, \033[1;32m" << username << "\033[0m!\n\n";
            return true;
        } else {
            error = errorColor + "Password incorrect.\n" + standard;
            return false;
        }
    }

    void printPrompt() {
        std::string prompt;
        if (currentColor != "\033[0;37m") {
            prompt = secColor + currentUser->name + "@GeistOS:" + currentDictonary + "$";
        } else {
            prompt = "\033[1;32m" + currentUser->name +
                    "@GeistOS\033[0m:\033[0;34m" +
                    currentDictonary + "\033[0m$";
        }

        std::cout << currentColor + prompt;
    }

    void run(std::string input) {

        std::cout << currentColor + "Type '\033[1;34mhelp\033[0m' for some Commands.\n\n";

        lastInput = std::chrono::steady_clock::now();

        //this line enables the Screen Saver
        //std::thread(idleMonitor).detach();


        while (std::getline(std::cin, input)) {
            try
            {
                std::this_thread::sleep_for(std::chrono::milliseconds(20));

                if (input == "NoColor") {
                    noColor = true;
                    std::cout << "No Color Mode activated";
                }
                else if (input == "exit")
                    break;

                if (!std::getline(std::cin, input)) {
                    std::cout << errorColor << "\nInput stream closed\n";
                    std::cin.clear();
                    continue;
                }

                if (input.empty())
                    continue;

                if (input.size() > 50000) {
                    std::cout << errorColor << "Input too large\n";
                    continue;
                }

                std::vector<std::string> pipeline;
                if (input.find('|') != std::string::npos)
                    pipeline = split(input, '|');
                else
                    pipeline.push_back(input);

                std::string pipeInput;
                bool error = false;

                for (const auto& raw : pipeline)
                {
                    if (error)
                        break;

                    std::string segment = trim(raw);
                    if (segment.empty())
                        continue;

                    auto tokens = split(segment, ' ');
                    if (tokens.empty())
                        continue;

                    bool sudoMode = false;

                    if (tokens[0] == "sudo") {
                        if (tokens.size() < 2) {
                            std::cout << sudoColor << "sudo" << standard << ": no command specified\n";
                            error = true;
                            break;
                        }

                        sudoMode = true;
                        tokens.erase(tokens.begin());
                    }

                    auto it = commands.find(tokens[0]);
                    if (it == commands.end()) {
                        std::cout << errorColor << "Unknown command" << standard << ": " << tokens[0] << "\n";
                        error = true;
                        break;
                    }

                    auto& cmd = it->second;

                    sysLog.addLog(input, cmd, currentUser);

                    if ((cmd.perm.Read && !handleRights(currentUser->userRights, 0)) ||
                        (cmd.perm.Write && !handleRights(currentUser->userRights, 1)) ||
                        (cmd.perm.Execute && !handleRights(currentUser->userRights, 2)))
                    {
                        std::cout << errorColor << "Permission denied\n";
                        error = true;
                        break;
                    }

                    std::string safeInput = pipeInput.substr(0, 10000);

                    if (sudoMode || cmd.perm.Sudo)
                    {
                        if (!handleRights(currentUser->userRights, 3)) {
                            std::cout << errorColor << "You have no Sudo Permission\n";
                            error = true;
                            break;
                        }

                        std::cout << "[" << sudoColor << "sudo" << standard << "] password: ";
                        std::string pass = getHiddenInput();

                        if (pass != currentUser->password) {
                            std::cout << errorColor << "Sorry, wrong Password.\n";
                            error = true;
                            break;
                        }
                    }

                    std::string output;

                    try {
                        output = cmd.func(tokens, safeInput);
                    } catch (const std::bad_alloc&) {
                        std::cout << errorColor << "Memory overflow in command: " << tokens[0] << "\n";
                        error = true;
                        break;
                    } catch (const std::exception& e) {
                        std::cout << errorColor << "Error in command: " << e.what() << "\n";
                        error = true;
                        break;
                    } catch (...) {
                        std::cout << errorColor << "Command crashed: " << tokens[0] << "\n";
                        error = true;
                        break;
                    }

                    pipeInput = output;

                    if (pipeInput.size() > 200000) {
                        std::cout << errorColor << "Pipe overflow protection triggered\n";
                        error = true;
                        break;
                    }
                }

                /*if (!error && !pipeInput.empty()) {
                    std::cout << pipeInput << std::endl;
                }*/
            } catch (const std::exception& e) {
                std::cout << "Fatal loop error: " << e.what() << "\n";
            } catch (...) {
                std::cout << "Unknown fatal error in main loop\n";
            }
        }
    }

    void stop() { running = false; }

    User* getCurrentUser() { return currentUser; }
    std::unordered_map<std::string, User>& getUsers() { return users; }
    void switchUser(const std::string& name) {
        if (users.find(name) != users.end()) {
            currentUser = &users[name];
        }
    }
};











class System : public Terminal {
private: 
    size_t columns;
    size_t rows;
    CONSOLE_SCREEN_BUFFER_INFO csbi;

    void calcScreenSize() {
        GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
        columns = csbi.srWindow.Right - csbi.srWindow.Left + 1;
        rows = csbi.srWindow.Bottom - csbi.srWindow.Top + 1;
    }

public: 
    void up(int n) {
        std::cout << "\x1b[" << n << "A"; // Move Up n Lines
    }
    void down(int n) {
        std::cout << "\x1b[" << n << "B"; // Move down n Lines
    }
    void right(int n) {
        std::cout << "\x1b[" << n << "C"; // Move Right n Lines
    }
    void left(int n) {
        std::cout << "\x1b[" << n << "D"; // Move Left n Lines
    }

    void saveCursorPos() {
        std::cout << "\x1b[s"; // Save Cursors Position
    }
    void restoreCursorPos() {
        std::cout << "\x1b[u"; // Restore Cursors Position
    }

    void refreshScreen() {
        calcScreenSize();

        std::cout << "\x1b[" << rows << "A"; // Move Cursor to Top of Terminal
    }

    void restoreScreen() {
        up(rows - 1);
        clearScreen();
    }

    void clearScreen() {
        calcScreenSize();

        for (size_t i = 0; i < (rows - 1); i++) {
            for (size_t u = 0; u < columns; u++)
                std::cout << " ";
            std::cout << "\n";
        }

        refreshScreen();
    }
};


System sys;














class ConsoleWindow {
public: 
    struct Row {
        std::string label;
        std::string value;
    };

    struct Section {
        std::string title;
        std::vector<Row> rows;
    };

    std::vector<Section> WindowConf = {};

    ConsoleWindow(int w, const std::string& t) : minWidth(w), title(t) {}

    void addConfig(std::vector<Section> config) {
        WindowConf = config;
    }

    void addSection(const std::string& sectionTitle = "") {
        sections.push_back({sectionTitle, {}});
    }

    void addRow(const std::string& label, const std::string& value) {
        if (sections.empty()) addSection();
        sections.back().rows.push_back({label, value});
    }

private:
    int minWidth;
    std::string title;
    std::vector<Section> sections;

    const std::string RESET   = "\033[0m";
    const std::string TITLE   = "\033[1;37m";
    const std::string BORDER  = "\033[1;36m";
    const std::string LABEL   = "\033[1;34m";
    const std::string VALUE   = "\033[1;32m";
    const std::string SECTION = "\033[1;33m";

    const std::string TL = "╔";
    const std::string TR = "╗";
    const std::string BL = "╚";
    const std::string BR = "╝";

    const std::string VL = "║";

    const std::string SL = "╠";
    const std::string SR = "╣";

    const std::string SSL = "╟";
    const std::string SSR = "╢";

    const std::string H  = "═";
    const std::string SH = "─";

    int calculateWidth() const {
        int w = minWidth;

        w = std::max(w, (int)title.size() + 4);

        for (const auto& sec : sections) {
            w = std::max(w, (int)sec.title.size() + 6);

            for (const auto& r : sec.rows) {
                int len = 2 + r.label.size() + 1 + r.value.size() + 2;
                w = std::max(w, len);
            }
        }

        return w;
    }

public:
    void render() {
        for (size_t i = 0; i < WindowConf.size(); i++) {
            addSection(WindowConf[i].title);
            for (const auto& row : WindowConf[i].rows) {
                addRow(row.label, row.value);
            }
        }

        draw();
    }

    void draw() const {
        int width = calculateWidth();

        auto repeat = [](const std::string& str, int count) {
            std::string result;
            result.reserve(str.size() * count);

            for (int i = 0; i < count; ++i)
                result += str;

            return result;
        };

        auto topLine = [&]() {
            std::cout << BORDER << TL
                    << repeat(H, width - 2)
                    << TR << RESET << "\n";
        };

        auto bottomLine = [&]() {
            std::cout << BORDER << BL
                    << repeat(H, width - 2)
                    << BR << RESET << "\n";
        };

        auto separatorLine = [&]() {
            std::cout << BORDER << SL
                    << repeat(H, width - 2)
                    << SR << RESET << "\n";
        };

        auto thinSeparator = [&]() {
            std::cout << BORDER << SSL
                    << repeat(SH, width - 2)
                    << SSR << RESET << "\n";
        };

        auto centered = [&](const std::string& text, const std::string& color) {
            int pad = (width - 2 - (int)text.size()) / 2;

            std::cout << BORDER << VL
                    << repeat(" ", pad)
                    << color << text << RESET
                    << repeat(" ", width - 2 - pad - text.size())
                    << BORDER << VL
                    << RESET << "\n";
        };

        auto sectionLine = [&](const std::string& text) {
            std::string t = " " + text + " ";

            int total = width - 2;
            int left = (total - (int)t.size()) / 2;
            int right = total - (int)t.size() - left;

            std::cout << BORDER << SSL
                    << repeat(SH, left)
                    << SECTION << t << RESET
                    << BORDER
                    << repeat(SH, right)
                    << SSR
                    << RESET << "\n";
        };

        auto row = [&](const Row& r) {
            std::ostringstream out;

            out << ' '
                << LABEL << std::left << std::setw(18) << r.label << RESET
                << ' '
                << VALUE << std::left << std::setw(width - 23) << r.value << RESET
                << ' ';

            std::string s = out.str();

            if ((int)s.size() < width - 2)
                s += repeat(" ", width - 2 - s.size());

            std::cout << BORDER << VL << RESET
                    << s
                    << BORDER << VL << RESET
                    << "\n";
        };

        topLine();
        centered(" " + title + " ", TITLE);
        separatorLine();

        for (const auto& sec : sections) {
            if (!sec.title.empty())
                sectionLine(sec.title);
            else
                thinSeparator();

            for (const auto& r : sec.rows)
                row(r);
        }

        bottomLine();
    }
};

class Config {
protected: 
    struct cmdHelp {
        std::string cmd;
        std::vector<std::string> args = {};
        boolean sudo = false;
        std::string suffix = ""; 
    };

    struct cmdEntry {
        cmdHelp help;
        std::string description = "";
    };

    struct Category {
        std::string title;
        std::vector<cmdEntry> lines;
        size_t width = 0;
    };





    struct changes {
        std::string type;
        std::string color;
        std::string description;
    };

    struct SubVersions {
        std::string title;
        std::vector<changes> changes;
    };

    struct versionHistory {
        std::string title;
        std::vector<SubVersions> subVersions;
    };



    

    struct Option {
        std::string flag;
        std::string description;
    };

    struct Example {
        std::string example;
    };

    struct ManPage {
        std::string name;
        std::string description;
        std::string usage;
        std::vector<Option> options;
        std::vector<Example> examples;
    };



    std::string standard        = "\033[0m";
    std::string addedColor      = getAnsiColor('1');
    std::string reworkColor     = getAnsiColor('8');
    std::string infoColor       = getAnsiColor('2');
    std::string disabledColor   = getAnsiColor('4');
    
private: 
    std::vector<Category> categories;
    std::vector<versionHistory> allVersions;
    std::vector<ManPage> manPage;

    void initHelpCategories() {
        categories.clear();

        categories.push_back({
            "General",
            {
                {"help"}, {"clear"}, {"echo"}, {"exit"}, {"logout"}, {"date"}
            }
        });

        categories.push_back({
            "Network & System",
            {
                {
                    {"ping", {"ip"}},
                    "Ping any Ip Adress"
                },
                {
                    {"ls", {"-s"}},
                    "Show the content of the current Directory, with -s also show File sizes"
                },
                {
                    {"db", {"create", "insert", "query", "list", "drop", "clear"}},
                    "Create a local Database and manage it with the given Commands"
                },
                {
                    {"sys", {"info", "help", "uptime", "time", "tasks", "run", "kill", "mem", "cpu", "host", "config", "update", "clearcache", "bench"}}, 
                    "Manage this System"
                }, 
                {
                    {"man", {"command"}},
                    "View the Manual of each Command"
                }
            }
        });

        categories.push_back({
            "File System",
            {
                {
                    {"dir /s"},
                    "Show All Folders"
                },
                {
                    {"cd", {"Folder Path", "Folder Name"}},
                    "Change the Current Directory"
                },
                {
                    {"mkdir", {"Folder Name"}},
                    "Create a new Folder"
                },
                {
                    {"rm", {"File Name", "Folder Name"}, true},
                    "Delete a selected File/Folder"
                },
                {
                    {"touch", {"File Name"}},
                    "Create a new File"
                },
                {
                    {"vim", {"File Name"}},
                    "Edit the selected File"
                },
                {
                    {"cat", {"File Name"}},
                    "Show the content of the File"
                },
                {
                    {"script", {"File Name"}},
                    "Execute the GeistScript you wrote"
                }
            }
        });

        categories.push_back({
            "Users & Permissions",
            {
                {
                    {"user", {"list", "add", "edit", "del", "help"}, true}, 
                    "Manage the Users of this System"
                },
                {
                    {"perm", {"list", "edit", "info", "help"}, true}, 
                    "Manage the Permissions of this System"
                },
                {
                    {"passwd", {}, true}, 
                    "Change the Password"
                }
            }
        });

        categories.push_back({
            "System Tools",
            {
                {
                    {"apt update/install", {"Package"}, true}, 
                    "Install/Update the selected Package"
                }
            }
        });

        categories.push_back({
            "Entertaining",
            {
                {
                    {"games", {"casino"}},
                    "Play the Games from this Category" 
                }, 
                {
                    {"bank", {}}, 
                    "Manage your simulated Finances"
                }
            }
        });

        categories.push_back({
            "UI & Output",
            {
                {
                    {"draw", {"frame", "square", "triangle"}}, 
                    "Draw some Shapes in the Terminal"
                },
                {
                    {"print", {"text"}}, 
                    "Print any Text in the Big Letters"
                },
                {
                    {"color", {"hex-codes"}, false, "(0-F, 7 default)"}, 
                    "Change the Color of the Text"
                }
            }
        });
    }




    void initManPages() {
        manPage.clear();

        manPage.push_back({
            "help",
            "Display help information",
            "help",
            {},
            {
                {"help"}
            }
        });

        manPage.push_back({
            "clear",
            "Clear the terminal screen",
            "clear",
            {},
            {
                {"clear"}
            }
        });

        manPage.push_back({
            "echo",
            "Display a line of text",
            "echo <text>",
            {
                {"<text>", "The text you want to display"}
            },
            {
                {"echo Hello, World!"},
                {"echo 'Hello World'"}
            }
        });

        manPage.push_back({
            "ls",
            "List directory contents",
            "ls [options]",
            {},
            {
                {"ls"},
                {"ls -la"}
            }
        });

        manPage.push_back({
            "exit",
            "Leave the Terminal",
            "exit",
            {},
            {
                {"exit"}
            }
        });

        manPage.push_back({
            "logout",
            "Logout the current user",
            "logout",
            {},
            {
                {"logout"}
            }
        });

        manPage.push_back({
            "date",
            "Show the current date and time",
            "date",
            {},
            {
                {"date"}
            }
        });

        manPage.push_back({
            "ping",
            "Ping any Ip Adress",
            "ping <ip>",
            {
                {"<ip>", "The IP address to ping"}
            },
            {
                {"ping 192.168.1.1"}
            }
        });

        manPage.push_back({
            "db",
            "Create a local Database and manage it with the given Commands",
            "db <create/insert/query/list/drop/clear>",
            {
                {"create", "Create a new table"},
                {"insert", "Insert data into a table"},
                {"query", "Query data from a table"},
                {"list", "List all tables"},
                {"drop", "Drop a table"},
                {"clear", "Clear all data from a table"}
            },
            {
                {"db create <tablename> <col1> <col2> ..."},
                {"db insert <tablename> <data>"},
                {"db query <tablename> where <condition>"},
                {"db list"},
                {"db drop <tablename>"},
                {"db clear <tablename>"}
            }
        });

        manPage.push_back({
            "sys",
            "Manage this System",
            "sys <info/help/uptime/time/tasks/run/kill/mem/cpu/host/config/update/clearcache/bench>",
            {
                {"info", "Show system information"},
                {"help", "Show the help message"},
                {"uptime", "Show how long the system is running"},
                {"time", "Show the current system time"},
                {"tasks", "Show all running tasks"},
                {"run", "Run a new task"},
                {"kill", "Kill a running task"},
                {"mem", "Show memory usage"},
                {"cpu", "Show CPU usage"},
                {"host", "Show host information"},
                {"config", "Show system configuration"},
                {"update", "Update the system (simulated)"},
                {"clearcache", "Clear the system cache (simulated)"},
                {"bench", "Run a benchmark test (simulated)"}
            },
            {
                {"sys info"},
                {"sys help"},
                {"sys uptime"},
                {"sys time"},
                {"sys tasks"},
                {"sys run <taskname>"},
                {"sys kill <pid>"},
                {"sys mem"},
                {"sys cpu"},
                {"sys host"},
                {"sys config"},
                {"sys update"},
                {"sys clearcache"},
                {"sys bench"}
            }
        });

        manPage.push_back({
            "man",
            "Show manual pages",
            "man <command>",
            {
                {"<command>", "The command for which to display help"}
            },
            {
                {"man ls"},
                {"man echo"},
                {"man"}
            }
        });

        manPage.push_back({
            "dir /s",
            "Show all folders",
            "dir /s",
            {},
            {
                {"dir /s"}
            }
        });

        manPage.push_back({
            "cd",
            "Change directory",
            "cd <Folderpath/Foldername>",
            {
                {"<Folderpath>", "The path of the folder you want to enter (e.g. /home/user/documents)"},
                {"<Foldername>", "The name of the folder you want to enter (e.g. documents)"}
            },
            {
                {"cd /home"},
                {"cd myFolder"}
            }
        });

        manPage.push_back({
            "mkdir",
            "Create a new folder",
            "mkdir <foldername>",
            {
                {"<foldername>", "The name of the folder you want to create"}
            },
            {
                {"mkdir myFolder"}
            }
        });

        manPage.push_back({
            "rm",
            "Remove file or directory",
            "rm <filename/foldername>",
            {
                {"<filename>", "The name of the file you want to remove"},
                {"<foldername>", "The name of the folder you want to remove"}
            },
            {
                {"rm file.txt"},
                {"rm myFolder"}
            }
        });

        manPage.push_back({
            "touch",
            "Create a new file",
            "touch <file>",
            {
                {"<file>", "The name of the file you want to create"}
            },
            {
                {"touch file.txt"}
            }
        });

        manPage.push_back({
            "vim",
            "Edit file contents",
            "vim <file>",
            {
                {"<file>", "The name of the file you want to edit"}
            },
            {
                {"vim file.txt"}
            }
        });

        manPage.push_back({
            "cat",
            "View file contents",
            "cat <file>",
            {
                {"<file>", "The name of the file you want to view"}
            },
            {
                {"cat file.txt"}
            }
        });

        manPage.push_back({
            "script",
            "Execute the GeistScript you wrote",
            "script <file>",
            {
                {"<file>", "The name of the script file you want to execute"}
            },
            {
                {"script file.txt"}
            }
        });

        manPage.push_back({
            "user",
            "Manage users of this System",
            "user <list/add/edit/del/help>",
            {
                {"list", "List all users"},
                {"add", "Add a new user"},
                {"edit", "Edit a user"},
                {"del", "Delete a user"},
                {"help", "Show this help message"}
            },
            {
                {"user list"},
                {"user add"},
                {"user edit"},
                {"user del"},
                {"user help"}
            }
        });

        manPage.push_back({
            "perm",
            "Manage permissions of this System",
            "perm <list/edit/info/help>",
            {
                {"list", "List all permissions"},
                {"edit", "Edit a permission"},
                {"info", "Show info about all permissions"},
                {"help", "Show this help message"}
            },
            {
                {"perm list"},
                {"perm edit"},
                {"perm info"},
                {"perm help"}
            }
        });

        manPage.push_back({
            "passwd",
            "Change password",
            "passwd",
            {},
            {
                {"passwd"}
            }
        });

        manPage.push_back({
            "apt update/install",
            "Install/Update Packages",
            "apt update/install <package>",
            {
                {"<package>", "The name of the package you want to install/update"}
            },
            {
                {"apt update"},
                {"apt install <package>"}
            }
        });

        manPage.push_back({
            "games",
            "Play the Games from this Category",
            "games <section>",
            {
                {"<section>", "The Section of the Games you want to play (e.g. casino)"}
            },
            {
                {"games casino"}
            }
        });

        manPage.push_back({
            "bank",
            "Manage your simulated Finances",
            "bank",
            {},
            {
                {"bank"}
            }
        });

        manPage.push_back({
            "draw",
            "Draw some Shapes in the Terminal",
            "draw <shape> <size>",
            {
                {"<shape>", "The type of shape you want to draw"},
                {"<size>", "The size of the shape you want to draw"}
            },
            {
                {"draw frame"},
                {"draw square 10"},
                {"draw square filled 10"},
                {"draw triangle 20"},
                {"draw triangle filled 20"}
            }
        });

        manPage.push_back({
            "print",
            "Print any Text in the Big Letters",
            "print <text>",
            {
                {"<text>", "The Text you want to print"}
            },
            {
                {"print 'Hello, World!'"}
            }
        });

        manPage.push_back({
            "color",
            "Change the Color of the Text",
            "color <hex-code>",
            {
                {"<hex-code>", "Hexadecimal color code"}
            },
            {
                {"color 1"}
            }
        });
    }





    void initVersions() {
        allVersions.clear();

        allVersions.push_back({
            "0.1",
            {
                {
                    "0.1.1",
                    {
                        {"Added", addedColor, "Dynamic C++ Code for an Linux like OS"}
                    }
                },
                {
                    "0.1.2",
                    {
                        {"Added", addedColor, "help\033[0m, \033[1;34mclear\033[0m, \033[1;34mecho\033[0m, \033[1;34mls\033[0m, \033[1;34mexit"},
                        {"Added", addedColor, "ping <ip>"}
                    }
                }
            }
        });

        allVersions.push_back({
            "0.2",
            {
                {
                    "0.2.1",
                    {
                        {"Added", addedColor, "dir /s"},
                        {"Added", addedColor, "apt update/install <Package>\033[0m (\033[1;36msudo\033[0m required)"}
                    }
                },
                {
                    "0.2.2",
                    {
                        {"Added", addedColor, "cd <Folder Path or Folder Name>"},
                        {"Added", addedColor, "mkdir <Folder Name>"}, 
                        {"Added", addedColor, "rm <filename|foldername>\033[0m (\033[1;36msudo\033[0m required)"}
                    }
                },
                {
                    "0.2.3",
                    {
                        {"Added", addedColor, "touch <File Name>"},
                        {"Added", addedColor, "vim <File Name>"}
                    }
                }
            }
        });

        allVersions.push_back({
            "0.3",
            {
                {
                    "0.3.1", 
                    {
                        {"Added", addedColor, "color <hex-code>\033[0m (0-F) {7 = default}"}
                    }
                },
                {
                    "0.3.2",
                    {
                        {"Added", addedColor, "addUser <Username>"},
                        {"Added", addedColor, "listUser <Username>"},
                        {"Added", addedColor, "delUser <Username>"},
                        {"Added", addedColor, "passwd\033[0m (\033[1;36msudo\033[0m required)"}
                    }
                }
            }
        });

        allVersions.push_back({
            "0.4",
            {
                {
                    "0.4.1", 
                    {
                        {"Reworked", reworkColor, "user <list/add/edit/del/help>\033[0m (\033[1;36msudo\033[0m required)"},
                        {"Reworked", reworkColor, "perm <list/edit/info/help>\033[0m (\033[1;36msudo\033[0m required)"}
                    }
                },
                {
                    "0.4.2",
                    {
                        {"Added", addedColor, "Spinning Ghost Idle Animation"},
                        {"Added", addedColor, "Early Versions of GeistOS GUI"},
                        {"Added", addedColor, "win"}
                    }
                },
                {
                    "0.4.3",
                    {
                        {"Added", addedColor, "Letter Library for print Cmd"},
                        {"Added", addedColor, "print <word to print>"}
                    }
                }
            }
        });

        allVersions.push_back({
            "0.5",
            {
                {
                    "0.5.1",
                    {
                        {"Reworked", reworkColor, "Spinning Ghost Idle Animation"}, 
                        {"Disabled", disabledColor, "Spinning 3D Ghost Idle Animation"}
                    }
                },
                {
                    "0.5.2",
                    {
                        {"Added", addedColor, "sys version <history/cur>"},
                        {"Added", addedColor, "sys log <show/clear>"}
                    }
                }
            }
        });

        allVersions.push_back({
            "0.6",
            {
                {
                    "0.6.1",
                    {
                        {"Reworked", reworkColor, "User Rank System"},
                        {"Reworked", reworkColor, "User Settings Menu"},
                        {"Added", addedColor, "A lot of Features to the sys Command"},
                        {"Info", infoColor, "Type 'sys help' to see all features"},
                        {"Added", addedColor, "Pipes that run multiple commands at once"}
                    }
                },
                {
                    "0.6.2",
                    {
                        {"Reworked", reworkColor, "The Help Screen with fresh colors and new Backend"},
                        {"Added", addedColor, "The Date Command with a beautiful Table View"},
                        {"Reworked", reworkColor, "The Date Command with a modular Design"}
                    }
                },
                {
                    "0.6.3",
                    {
                        {"Reworked", reworkColor, "The Logic of the 'help' and the 'sys versions' command to be more modular"},
                        {"Added", addedColor, "A central Config class that the 'help' and 'sys version' command get the data from"}
                    }
                },
                {
                    "0.6.4",
                    {
                        {"Added", addedColor, "A new 'games' command that has different groups of games"},
                        {"Added", addedColor, "The section 'casino' to the 'games' command"},
                        {"Added", addedColor, "The Games 'roulette', 'Dice Rolling Game' and 'Slots' to the 'games casino' command"},
                        {"Added", addedColor, "A new 'bank' command"}
                    }
                },
                {
                    "0.6.5",
                    {
                        {"Reworked", reworkColor, "The 'sys version history' command with fresh visuals and clear versions and subVersions"},
                        {"Reworked", reworkColor, "The Logic of the main function to be more modular and easy to modify"}
                    }
                },
                {
                    "0.6.6",
                    {
                        {"Added", addedColor, "The AsciiGraph Class to manage and render Graphes in pure Text"},
                        {"Added", addedColor, "A new 'graph' command to test the AsciiGraph Class"},
                        {"Added", addedColor, "The ProgressBar Class to add Progressbars with custom width"},
                        {"Reworked", reworkColor, "The Logic of the 'sys log' command to save the commands with timestamps in a vector"}
                    }
                }, 
                {
                    "0.6.7", 
                    {
                        {"Fixed", infoColor, "The Logic of the main function to be more modular and easy to modify"}, 
                        {"Added", addedColor, "A new 'cat' command in Order to view the content of a file in the terminal"},
                        {"Reworked", reworkColor, "The 'ls' command to be cleaner and have fresh colors"},
                        {"Reworked", reworkColor, "The 'help' command to show a small Description for each command"},
                        {"Added", addedColor, "The 'db' command to manage a local simulated Database"},
                        {"Reworked", reworkColor, "The Version numbering system from 0.0.0.1 to 0.1"},
                        {"Added", addedColor, "The 'man' command to see how to use each command with examples and options"},
                        {"Reworked", reworkColor, "The Manual Entries for some commands to be more informative and have examples"}
                    }
                }, 
                {
                    "0.6.8", 
                    {
                        {"Reworked", reworkColor, "The 'vim' command to dynamically expand width and to be more appealing"},
                        {"Reworked", reworkColor, "The ConsoleWindow Class to use the uniCode Characters to render a beautiful Window"},
                        {"Reworked", reworkColor, "The ConsoleWindow Class to now take a vector of Sections with each Section having a vector of Rows, to be more modular and easy to use"},
                        {"Reworked", reworkColor, "The Log System to now save the Logs with what user executed the command and to dynamically display them"},
                        {"Reworked", reworkColor, "The 'vim' command to now display the command list and the user input at the bottom of the window"},
                        {"Added", addedColor, "The 'draw' command to draw different kind of shapes, for now you can only draw a frame around the terminal window"},
                        {"Reworked", reworkColor, "The 'draw square' command to now print the square in a correct formatted style"},
                        {"Reworked", reworkColor, "The 'draw' command to now have a dynamic shape drawing algorythm and a new logic of choosing a shape"},
                        {"Reworked", reworkColor, "The shape managing algorythm of the 'draw' command to now print clear Errors"}
                    }
                }, {
                    "0.6.9", 
                    {
                        {"Reworked", reworkColor, "The SysLog Class to now Log more Data about the command you run"},
                        {"Reworked", reworkColor, "The SysLog Class to now store the logged Data in a vector with clear Struct names"},
                        {"Reworked", reworkColor, "All the Commands to now have the correct rights asigned needed to run them"},
                        {"Reworked", reworkColor, "The standard User saving algorythm to now store them in a vector and then initialise them"},
                        {"Reworked", reworkColor, "The 'db' command to now print using the ASCII Lines in order to print the database border"},
                        {"Reworked", reworkColor, "The 'db' command to now only accept == as condition and no longer just = as a condition"},
                        {"Reworked", reworkColor, "The clearScreen function to now have its own System Class and to be dynamic"},
                        {"Added", addedColor, "The 'script' command to execute GeistScript Files and view the output in the Terminal"},
                        {"Added", addedColor, "Variable support in GeistScript"},
                        {"Added", addedColor, "let and const Variables in GeistScript"},
                        {"Added", addedColor, "Function support in GeistScript"},
                        {"Added", addedColor, "Parameter support to the functions in GeistScript"},
                        {"Added", addedColor, "Arithmetic variable Value calculation support in GeistScript"},
                        {"Reworked", reworkColor, "The initialisation of the Arithmetic handler Function"},
                        {"Reworked", reworkColor, "The Error messages shown by the GeistScript Interpreter"},
                        {"Added", addedColor, "If, While and For Loop support in GeistScript"},
                        {"Added", addedColor, "Else If and Else support in GeistScript"},
                        {"Added", addedColor, "Short form Arithmetics such as +=, -=, *=, /= and %="},
                        {"Reworked", reworkColor, "The script command to now only take .gsScript files as an input"},
                        {"Added", addedColor, "Very early Versions of Helper Functions for the Class support in GeistScript"}
                    }
                }
            }
        });
    }

public: 
    Config() {
        initHelpCategories();
        initVersions();
        initManPages();
    }

    std::vector<Category> getHelpCategories() {
        return categories;
    }

    std::vector<versionHistory> getAllVersions() {
        return allVersions;
    }

    std::vector<ManPage> getManPage() {
        return manPage;
    }
};

Config config;

class Help: public Config {
private:
    int maxPerLine = 4;
    int maxListPerLine = 15;
    const size_t HELP_COLUMN = 50;

    std::string standard   = getAnsiColor('8');
    std::string textColor1 = getAnsiColor('1');
    std::string textColor2 = getAnsiColor('3');
    std::string errorColor = getAnsiColor('4');
    std::string catTitle   = getAnsiColor('D');
    std::string sudoColor  = getAnsiColor('B');

    std::vector<Category> categories;

    void printCategory(const std::string& title) {
        std::cout << "\n" << standard << "==== " << catTitle << title << standard << " ====\n";
    }

    void printStandardCmds(const std::vector<cmdEntry>& cmds) {
        std::cout << textColor1 << "  ";

        for (size_t i = 0; i < cmds.size(); i++) {
            if (i != 0 && i % maxPerLine == 0) {
                std::cout << "\n  ";
            }

            std::cout << textColor1 << cmds[i].help.cmd;

            if (i < cmds.size() - 1 && ((i + 1) % maxPerLine) != 0) {
                std::cout << standard << ", ";
            }
        }

        std::cout << standard << "\n";
    }

    void printCmds() {
        int catIndex = 0;
        for (const auto& category : categories) {
            printCategory(category.title);

            if (catIndex > 0) {
                for (const auto& cmd : category.lines) {
                    printHelp(cmd.help.cmd, cmd.help.args, cmd.help.sudo, cmd.help.suffix, false, cmd.description);
                }
            } else {
                printStandardCmds(category.lines);
            }

            catIndex++;
        }
    }

    void initCategories() {
        categories = config.getHelpCategories();
    }

public:
    void printHelp(
        const std::string& cmd,
        const std::vector<std::string>& list,
        bool sudo = false,
        const std::string& suffix = "",
        bool isUsage = false,
        std::string desc = ""
    ) {
        size_t visibleLength = 0;

        if (!isUsage) {
            std::cout << textColor1 << "  " << cmd;
            visibleLength = 2 + cmd.size();
        } else {
            std::cout << errorColor << "Invalid Usage!\n"
                    << errorColor << "Usage"
                    << standard   << ":"
                    << textColor1 << " " << cmd;
            visibleLength = 6 + cmd.size();
        }

        std::vector<std::vector<std::string>> lines;

        if (!list.empty()) {
            std::vector<std::string> current;
            size_t len = 0;

            for (size_t i = 0; i < list.size(); i++) {
                size_t add = list[i].size();
                size_t extra = current.empty() ? 0 : 1;

                if (!current.empty() &&
                    ((int)(current.size()) >= maxListPerLine ||
                    (int)(len + add + extra) >= ((maxListPerLine - 1) * 2))) {

                    lines.push_back(current);
                    current.clear();
                    len = 0;
                }

                current.push_back(list[i]);
                len += add + extra;
            }

            if (!current.empty()) {
                lines.push_back(current);
            }
        }

        if (!lines.empty()) {
            std::cout << standard << " <";
            visibleLength += 2;

            for (size_t i = 0; i < lines[0].size(); i++) {
                if (i > 0) {
                    std::cout << standard << "/";
                    visibleLength++;
                }

                std::cout << textColor2 << lines[0][i];
                visibleLength += lines[0][i].size();
            }

            if (lines.size() > 1) {
                std::cout << standard << "/";
                visibleLength++;
            } else {
                std::cout << standard << ">";
                visibleLength++;
            }
        }

        if (sudo) {
            std::string sudoText = " ("; 
            sudoText += "sudo";
            sudoText += " required)";

            std::cout << standard << " (" << sudoColor << "sudo" << standard << " required)";
            visibleLength += sudoText.size();
        }

        if (!suffix.empty()) {
            std::cout << standard << " " << suffix;
            visibleLength += 1 + suffix.size();
        }

        if (!desc.empty()) {
            if (visibleLength < HELP_COLUMN) {
                std::cout << std::string(HELP_COLUMN - visibleLength, ' ');
            } else {
                std::cout << " ";
            }

            std::cout << standard << desc;
        }

        std::cout << "\n";

        if (lines.size() > 1) {
            size_t indent;
            if (!isUsage) indent = cmd.size() + 2;
            else indent = cmd.size() + 7; 

            for (size_t l = 1; l < lines.size(); l++) {
                std::cout << "  ";
                for (size_t i = 0; i < indent; i++) {
                    std::cout << " ";
                }

                for (size_t i = 0; i < lines[l].size(); i++) {
                    if (i > 0) {
                        std::cout << standard << "/";
                    }

                    std::cout << textColor2 << lines[l][i];
                }

                if (l < lines.size() - 1) {
                    std::cout << standard << "/";
                } else {
                    std::cout << standard << ">";
                }

                std::cout << "\n";
            }
        }

        if (isUsage) {
            std::cout   << errorColor << "If you don't know how to use this command, type: " 
                        << textColor1 << "man " << cmd << "\n";
        }
    }

    void render() {
        initCategories();
        printCmds();
    }
};

Help help;

void cmd_help(const std::vector<std::string>& args) {
    (void)args;

    printScreen("Help");
    std::cout << currentColor + "Available Commands:\n";
    help.render();
}

void cmd_clear(const std::vector<std::string>& args) {
    (void)args;
    #if defined(_WIN32)
        sys.clearScreen();
    #else
        system("clear");
    #endif
}

void cmd_echo(const std::vector<std::string>& args) {
    for (size_t i = 1; i < args.size(); i++) {
        std::cout << currentColor + args[i] << " ";
    }
    std::cout << currentColor + "\n";
}

void cmd_color(const std::vector<std::string>& args) {
    if (args.size() < 2) {
        help.printHelp("color", {"hex-code"}, false, "(0-F)", true);
        return;
    }

    char code = args[1][0]; // nur 1. Zeichen
    currentColor = getAnsiColor(code);
    secColor = getAnsiColor(code);

    std::cout << currentColor + currentColor << "Color changed to code " << code << "\n";
}

void cmd_ping(const std::vector<std::string>& args) {
    std::string command;

    #ifdef _WIN32
        // Windows-Befehl
        command = "ping " + args[1];
    #else
        // Linux / Mac-Befehl
        command = "ping -c 4 " + args[0];
    #endif

    std::cout << currentColor + "Running Ping...\n\n";
    int result = system(command.c_str());

    if(result == 0) {
        std::cout << currentColor + "\nPing successful!\n";
    } else {
        std::cout << currentColor + "\nPing Error!\n";
    }
}

void cmd_dir(const std::vector<std::string>& args) {
    // Beispiel: dir mit mehreren Argumenten
    std::string path = ".";       // aktuelles Verzeichnis
    std::string attributes = "/a"; // Alle Dateien anzeigen (inkl. versteckte)
    std::string recursive = "/s";  // rekursiv
    std::string bare = "/b";       // nur Pfade ohne Header/Footer
    std::string order = "/o:n";    // sortieren nach Name
    std::string pause = "";        // optional /p für Pause, leer = keine Pause

    std::string command = "dir " + args[1];

    std::cout << currentColor + "Running Command: " << command << "\n\n";

    // popen startet den Befehl und liefert die Ausgabe
    std::unique_ptr<FILE, decltype(&_pclose)> pipe(_popen(command.c_str(), "r"), _pclose);
    if (!pipe) {
        std::cerr << currentColor + "Error while running Command!\n";
    }

    char buffer[256];
    while (fgets(buffer, sizeof(buffer), pipe.get()) != nullptr) {
        std::cout << currentColor + buffer; // Ausgabe zeilenweise
    }
}


void wait(int ms) {
    std::this_thread::sleep_for(std::chrono::milliseconds(ms));
}

std::string fakeIP(const std::string& pkg) {
    return "https://" + pkg + ".com/" 
           + std::to_string(rand() % 256) + "." 
           + std::to_string(rand() % 256) + "." 
           + std::to_string(rand() % 256) + "." 
           + std::to_string(rand() % 256);
}

// Zufallszahlen einsetzen anstelle von Zahlen & %d
std::string replaceWithRandom(const std::string& input, std::mt19937& gen) {
    std::string result = input;

    // 1. %d durch kleine Zufallszahlen ersetzen (1-20)
    std::uniform_int_distribution<> smallDist(1, 20);
    std::regex percentRegex("%d");
    result = std::regex_replace(result, percentRegex, std::to_string(smallDist(gen)));

    // 2. vorhandene Zahlen durch größere Zufallszahlen ersetzen (500-9999)
    std::uniform_int_distribution<> largeDist(500, 9999);
    std::regex numberRegex("\\d+");
    auto words_begin = std::sregex_iterator(result.begin(), result.end(), numberRegex);
    auto words_end = std::sregex_iterator();

    std::string finalResult;
    size_t lastPos = 0;
    for (auto it = words_begin; it != words_end; ++it) {
        finalResult += result.substr(lastPos, it->position() - lastPos);
        finalResult += std::to_string(largeDist(gen));
        lastPos = it->position() + it->length();
    }
    finalResult += result.substr(lastPos);

    return finalResult;
}

int randomNum(int min, int max) {
    // Zufallsgenerator initialisieren
    static std::random_device rd;   // Zufallssamen
    static std::mt19937 gen(rd());  // Mersenne-Twister-Generator

    // Gleichverteilte Ganzzahl im Bereich [min, max]
    std::uniform_int_distribution<> dist(min, max);
    return dist(gen);
}


void cmd_apt_install(const std::vector<std::string>& args) {
    if (args.size() < 2 || args.size() > 4) {
        help.printHelp("sudo apt update/install", {"package"}, false, "", true);
        return;
    }

    std::string pkg = args[2];
    srand(time(0));

    if (args[1] == "install") {
        std::cout << currentColor + "Reading package lists... Done" << std::endl;
        wait(700);
        std::cout << currentColor + "Building dependency tree" << std::endl;
        wait(700);
        std::cout << currentColor + "Reading state information... Done" << std::endl;
        wait(700);
        std::cout << currentColor + "The following NEW packages will be installed:" << std::endl;
        std::cout << currentColor + "  " << pkg << std::endl;
        wait(700);
        std::cout << currentColor + "0 upgraded, 1 newly installed, 0 to remove and 0 not upgraded." << std::endl;
        std::cout << currentColor + "Need to get " + std::to_string(rand() % 20) + " MB of archives." << std::endl;
        std::cout << currentColor + "After this operation, " + std::to_string(rand() % 20) + " MB of additional disk space will be used." << std::endl;
        wait(1000);

        std::cout << currentColor + "Get:1 " << fakeIP(pkg) << " " << pkg << " [" + std::to_string(rand() % 20) + " MB]" << std::endl;
        wait(1500);

        std::cout << currentColor + "Pulling packages (" << fakeIP(pkg) << ")" << std::endl;
        wait(1000);

        // Fortschrittsbalken
        for (int i = 0; i <= 100; i+=10) {
            std::cout << currentColor + "\rProgress: " << i << "%";
            std::cout.flush();
            wait(randomNum(500, 3000));
        }
        std::cout << std::endl;

        int fetch = rand() % 15000;
        int download = rand() % 15000;
        int sec = fetch / download;

        std::cout << currentColor + "Fetched " + std::to_string(fetch) + " kB in " + std::to_string(sec) + "s (" + std::to_string(download) + " kB/s)" << std::endl;
        wait(500);
        std::cout << currentColor + "Selecting previously unselected package " << pkg << "." << std::endl;
        wait(500);

        for (int i = 0; i <= 100; i+=5) {
            std::cout << currentColor + "\r(Reading database ... " << i << "% complete)";
            std::cout.flush();
            wait(randomNum(250, 3000));
        }
        std::cout << std::endl;

        wait(500);
        std::cout << currentColor + "Preparing to unpack ..." << std::endl;
        wait(500);
        std::cout << currentColor + "Unpacking " << pkg << " (" + std::to_string(rand() % 5) + "." + std::to_string(rand() % 6) + "-" + std::to_string(rand() % 9) + ") ..." << std::endl;
        wait(1000);
        std::cout << currentColor + "Setting up " << pkg << " (" + std::to_string(rand() % 5) + "." + std::to_string(rand() % 6) + "-" + std::to_string(rand() % 9) + ") ..." << std::endl;
        wait(800);
        std::cout << currentColor + "Processing triggers for man-db (" + std::to_string(rand() % 5) + "." + std::to_string(rand() % 6) + "-" + std::to_string(rand() % 9) + ") ..." << std::endl;

        std::cout << currentColor + "\nInstallation finished." << std::endl;
    } else if (args[1] == "update") {
        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_int_distribution<> sleepDist(1000, 6000); // Pause zwischen 1s und 6s

        std::vector<std::string> fakeMessages = {
            "Get:1 http://archive.ubuntu.com/ubuntu focal InRelease [265 kB]",
            "Get:2 http://archive.ubuntu.com/ubuntu focal-updates InRelease [123 kB]",
            "Get:3 http://archive.ubuntu.com/ubuntu focal-backports InRelease [48 kB]",
            "Get:4 http://security.ubuntu.com/ubuntu focal-security InRelease [110 kB]",
            "Get:5 http://security.ubuntu.com/ubuntu focal-workspace InRelease [170 kB]",
            "Get:6 http://security.ubuntu.com/ubuntu focal-frontend InRelease [210 kB]",
            "Get:7 http://security.ubuntu.com/ubuntu focal-backend InRelease [180 kB]",
            "Get:8 http://security.ubuntu.com/ubuntu focal-database InRelease [180 kB]",
            "Get:9 http://security.ubuntu.com/ubuntu focal-image InRelease [180 kB]",
            "Get:10 http://security.ubuntu.com/ubuntu focal-version InRelease [180 kB]",
            "Get:11 http://security.ubuntu.com/ubuntu focal-programm InRelease [180 kB]",
            "Get:12 http://security.ubuntu.com/ubuntu focal-snapshot InRelease [180 kB]",
            "Your System got %d kB in %d s (%d kB/s).",
            "Packages are being read... Finished"
        };

        for (auto& msg : fakeMessages) {
            std::string modified = replaceWithRandom(msg, gen);
            std::cout << currentColor + modified << std::endl;
            std::this_thread::sleep_for(std::chrono::milliseconds(sleepDist(gen)));
        }
    } else {
        help.printHelp("sudo apt update/install", {"package"}, false, "", true);
    }
}

void cmd_cd(const std::vector<std::string>& args) {
    if (args[1] != "/") {
        currentDictonary = args[1] + "/";
    } else {
        currentDictonary = args[1];
    }
}

std::string getExecutablePath() {
    #ifdef _WIN32
        char buffer[MAX_PATH];
        GetModuleFileNameA(NULL, buffer, MAX_PATH);
        return std::string(buffer);
    #else
        char buffer[PATH_MAX];
        ssize_t len = readlink("/proc/self/exe", buffer, sizeof(buffer) - 1);
        if (len != -1) {
            buffer[len] = '\0';
            return std::string(buffer);
        }
        return "";
    #endif
}

std::string getExecutableDir(const std::string &exePath) {
    #ifdef _WIN32
        size_t pos = exePath.find_last_of("\\/");
    #else
        size_t pos = exePath.find_last_of('/');
    #endif
        if (pos == std::string::npos) {
            return "."; // falls kein Trennzeichen gefunden wurde
        }
        return exePath.substr(0, pos);
}


void cmd_mkdir(const std::vector<std::string>& args) {
    // Pfad der .exe und Verzeichnis bestimmen
    std::string exePath = getExecutablePath();
    std::string exeDir  = getExecutableDir(exePath);

    // Zielordner zusammensetzen
    #ifdef _WIN32
        std::string ordnerName = exeDir + "\\" + currentDictonary + args[1];
    #else
        std::string ordnerName = exeDir + "/" + args[1];
    #endif

    #ifdef _WIN32
        if (CreateDirectoryA(ordnerName.c_str(), NULL) ||
            GetLastError() == ERROR_ALREADY_EXISTS) {
            std::cout << currentColor + "Created Folder or replaced existing one: " << ordnerName << std::endl;
        } else {
            std::cerr << currentColor + "Can't create the Folder!" << std::endl;
        }
    #else
        if (mkdir(ordnerName.c_str(), 0777) == 0) {
            std::cout << currentColor + "Created Folder: " << ordnerName << std::endl;
        } else if (errno == EEXIST) {
            std::cout << currentColor + "Folder already exists: " << ordnerName << std::endl;
        } else {
            perror("Can't create the Folder");
        }
    #endif
}

// Ordnerinhalt auflisten
void listDirectory(const std::string &path) {
    #ifdef _WIN32
        WIN32_FIND_DATAA findFileData;
        HANDLE hFind = FindFirstFileA((path + "\\*").c_str(), &findFileData);

        if (hFind == INVALID_HANDLE_VALUE) {
            std::cerr << currentColor + "Error while opening Folder: " << path << std::endl;
            return;
        }

        do {
            std::string name = findFileData.cFileName;
            if (name != "." && name != "..") {
                if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
                    std::cout << currentColor + "[Folder] " << name << std::endl;
                } else {
                    std::cout << currentColor + "[File]  " << name << std::endl;
                }
            }
        } while (FindNextFileA(hFind, &findFileData) != 0);

        FindClose(hFind);

    #else
        DIR *dir = opendir(path.c_str());
        if (!dir) {
            std::cerr << currentColor + "Error while opening Folder: " << path << std::endl;
            return;
        }

        struct dirent *entry;
        while ((entry = readdir(dir)) != nullptr) {
            std::string name = entry->d_name;
            if (name != "." && name != "..") {
                if (entry->d_type == DT_DIR) {
                    std::cout << currentColor + "[Folder] " << name << std::endl;
                } else {
                    std::cout << currentColor + "[File]  " << name << std::endl;
                }
            }
        }
        closedir(dir);
    #endif
}

std::string formatSize(ULONGLONG size) {
    const double KB = 1024.0;
    const double MB = KB * 1024;
    const double GB = MB * 1024;

    std::ostringstream out;
    out << std::fixed << std::setprecision(2);

    if (size >= GB)
        out << (size / GB) << " GB";
    else if (size >= MB)
        out << (size / MB) << " MB";
    else if (size >= KB)
        out << (size / KB) << " KB";
    else
        out << size << " B";

    return out.str();
}

std::string getExtension(const std::string& filename) {
    size_t pos = filename.find_last_of('.');
    if (pos == std::string::npos || pos == filename.length() - 1)
        return "FILE";

    std::string ext = filename.substr(pos + 1);

    std::transform(ext.begin(), ext.end(), ext.begin(), ::toupper);

    return ext;
}

class ListDirectory {
    private: 
        const std::string RESET = "\033[0m";
        const std::string BOLD = "\033[1m";
        const std::string CYAN2 = "\033[36m";
        const std::string GREEN = "\033[32m";
        const std::string GRAY = "\033[90m";

        void listDirectoryStyled(const std::string& path, bool printSize = false) {
            WIN32_FIND_DATA findFileData;
            HANDLE hFind;

            std::string fileSize = "";

            std::string searchPath = path + "\\*";
            hFind = FindFirstFile(searchPath.c_str(), &findFileData);

            if (hFind == INVALID_HANDLE_VALUE) {
                std::cout << "Error reading directory\n";
                return;
            }

            int fileCount = 0;
            int dirCount = 0;

            std::cout << BOLD
                    << std::left << std::setw(10) << "TYPE"
                    << std::setw(30) << "NAME"
                    << std::setw(12) << "SIZE"
                    << RESET << "\n";

            std::cout << "--------------------------------------------------------\n";

            do {
                std::string name = findFileData.cFileName;
                if (name == "." || name == "..") continue;

                bool isDir = findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY;

                std::cout << std::left;

                if (isDir) {
                    std::cout << CYAN2
                            << std::setw(10) << "[DIR]"
                            << std::setw(30) << name
                            << std::setw(12) << "-"
                            << RESET << "\n";
                    dirCount++;
                } else {
                    ULONGLONG size =
                        (static_cast<ULONGLONG>(findFileData.nFileSizeHigh) << 32) |
                        findFileData.nFileSizeLow;

                    if (printSize) fileSize = formatSize(size);

                    std::string ext = getExtension(name);
                    std::string label = "[" + ext + "]";

                    std::cout << GREEN
                            << std::setw(10) << label
                            << std::setw(30) << name
                            << std::setw(12) << fileSize
                            << RESET << "\n";

                    fileCount++;
                }

            } while (FindNextFile(hFind, &findFileData) != 0);

            FindClose(hFind);

            std::cout << "\n";
            std::cout << GRAY << "Folders: " << dirCount
                    << " | Files: " << fileCount << RESET << "\n";
        }

    public: 
        void printDirectory(bool printSize = false) {
            char buffer[1024];
            GetModuleFileName(NULL, buffer, sizeof(buffer));

            std::string exePath(buffer);
            exePath = exePath.substr(0, exePath.find_last_of("\\/"));

            std::string folderPath = exePath + "\\" + currentDictonary;

            std::cout << "\n";
            std::cout << BOLD << "=== DIRECTORY LIST ===" << RESET << "\n";
            std::cout << GRAY << "Path: " << RESET << folderPath << "\n";
            std::cout << "--------------------------------------------------------\n\n";

            listDirectoryStyled(folderPath, printSize);

            std::cout << "\n";
        }
};

void cmd_ls(const std::vector<std::string>& args) {
    ListDirectory lsCmd;

    if (args.size() < 2) lsCmd.printDirectory();
    else if (args[1] == "-s") lsCmd.printDirectory(true);
    else help.printHelp("ls", {"-s"}, false, "", true);
}

void cmd_rm(const std::vector<std::string>& args) {
    if (args.size() < 2) {
        help.printHelp("rm", {"filename", "foldername"}, false, "", true);
        return;
    }

    std::string targetName = args[1];
    char buffer[1024];

    #ifdef _WIN32
        // Pfad der exe herausfinden
        GetModuleFileNameA(NULL, buffer, sizeof(buffer));
        std::string exePfad(buffer);
        exePfad = exePfad.substr(0, exePfad.find_last_of("\\/"));

        std::string targetPath = exePfad + "\\" + currentDictonary + targetName;

        DWORD attr = GetFileAttributesA(targetPath.c_str());
        if (attr == INVALID_FILE_ATTRIBUTES) {
            std::cerr << currentColor + "Error: File or directory not found!" << std::endl;
            return;
        }

        if (attr & FILE_ATTRIBUTE_DIRECTORY) {
            // Ordner löschen
            if (RemoveDirectoryA(targetPath.c_str())) {
                std::cout << currentColor + "Deleted Folder: " << targetName << std::endl;
            } else {
                std::cerr << currentColor + "Error while deleting folder (is it empty?)" << std::endl;
            }
        } else {
            // Datei löschen
            if (DeleteFileA(targetPath.c_str())) {
                std::cout << currentColor + "Deleted File: " << targetName << std::endl;
            } else {
                std::cerr << currentColor + "Error while deleting file!" << std::endl;
            }
        }

    #else
        // Pfad der exe herausfinden
        ssize_t len = readlink("/proc/self/exe", buffer, sizeof(buffer)-1);
        if (len == -1) {
            perror("Can't find the path");
            return;
        }
        buffer[len] = '\0';
        std::string exePfad(buffer);
        exePfad = exePfad.substr(0, exePfad.find_last_of("\\/"));

        std::string targetPath = exePfad + "/" + targetName;

        struct stat pathStat;
        if (stat(targetPath.c_str(), &pathStat) != 0) {
            perror("Error: File or directory not found");
            return;
        }

        if (S_ISDIR(pathStat.st_mode)) {
            // Ordner löschen
            if (rmdir(targetPath.c_str()) == 0) {
                std::cout << currentColor + "Deleted Folder: " << targetPath << std::endl;
            } else {
                perror("Error while deleting folder (is it empty?)");
            }
        } else if (S_ISREG(pathStat.st_mode)) {
            // Datei löschen
            if (unlink(targetPath.c_str()) == 0) {
                std::cout << currentColor + "Deleted File: " << targetPath << std::endl;
            } else {
                perror("Error while deleting file");
            }
        } else {
            std::cerr << currentColor + "Error: Not a regular file or directory!" << std::endl;
        }
    #endif
}

void cmd_touch(const std::vector<std::string>& args) {
    (void)args;
    std::string filename = args[1];

    char buffer[1024];

    GetModuleFileName(NULL, buffer, sizeof(buffer));
    std::string exePath(buffer);
    exePath = exePath.substr(0, exePath.find_last_of("\\/"));

    std::string ordnerPfad = exePath + "\\" + currentDictonary;

    std::string filePath = ordnerPfad + filename;

    std::ofstream newFile(filePath);
    if (newFile.is_open()) {
        std::cout << currentColor + "Created File sucessfully: " << filePath << std::endl;
        newFile.close();
    } else {
        std::cerr << currentColor + "Error: Can't create File!" << std::endl;
    }
}

class Vim {
private:
    std::string filename;
    std::string filePath;
    std::vector<std::string> lines;
    int linesCount;
    int minHeight = 6;
    size_t contentWidth;

    CONSOLE_SCREEN_BUFFER_INFO csbi;
    int columns, rows;

    const std::string COLOR_BORDER = getAnsiColor('B');
    const std::string COLOR_LINE   = getAnsiColor('F');
    const std::string COLOR_CMD    = getAnsiColor('7');
    const std::string COLOR_ERROR  = getAnsiColor('C');
    const std::string COLOR_OK     = getAnsiColor('A');
    const std::string COLOR_INFO   = getAnsiColor('9');
    const std::string COLOR_DESC   = getAnsiColor('8');
    const std::string COLOR_RESET  = "\033[0m";

    struct CommandItem {
        std::string key;
        std::string desc;
    };

    std::vector<CommandItem> cmds = {
        {"w", "Save"},
        {"q", "Quit"},
        {"a", "Add"},
        {"e num", "Edit"},
        {"d num", "Delete"}
    };

    void printHorizontal(std::string left, std::string mid, std::string right) {
        std::cout << COLOR_BORDER << left;

        for (size_t i = 0; i < contentWidth; i++)
            std::cout << mid;

        std::cout << right << "\n" << COLOR_RESET;
    }

    void printSeparator(
        const std::string& start,
        const std::string& mid,
        const std::string& sep,
        const std::string& end,
        size_t gap,
        size_t /*extra*/)
    {
        std::cout << COLOR_BORDER << start;

        for (size_t i = 0; i < cmds.size(); ++i) {
            size_t cellWidth = 2 + cmds[i].key.length() + 2 + cmds[i].desc.length();
            size_t width = cellWidth + gap;

            if (i > 1) width--;

            for (size_t j = 0; j < width; ++j)
                std::cout << mid;

            if (i + 1 < cmds.size()) std::cout << COLOR_BORDER << sep;
            else std::cout << COLOR_BORDER << mid;
        }

        std::cout << COLOR_BORDER << end << '\n';
    }

    void calcScreenSize() {
        GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
        columns = csbi.srWindow.Right - csbi.srWindow.Left + 1;
        rows = csbi.srWindow.Bottom - csbi.srWindow.Top + 1;

        (void)columns;
    }

    void renderBottomBox() {
        calcScreenSize();

        linesCount += minHeight;

        for (int i = 0; i < (rows - linesCount); i++)
            std::cout << "\n";

        //printHorizontal("┌", "─", "┐");
        drawCmds(contentWidth);
        //printHorizontal("└", "─", "┘");
    }

public:
    Vim(const std::string& file)
        : filename(file) {
        char buffer[MAX_PATH];

        GetModuleFileNameA(NULL, buffer, sizeof(buffer));

        std::string exePath(buffer);
        exePath = exePath.substr(0, exePath.find_last_of("\\/"));

        filePath = exePath + "\\" + currentDictonary + filename;

        loadFile();
    }

    void loadFile() {
        lines.clear();

        std::ifstream in(filePath);

        if (!in.is_open())
            return;

        std::string line;

        while (std::getline(in, line))
            lines.push_back(line);

        in.close();
    }

    void saveFile() {
        std::ofstream out(filePath, std::ios::trunc);

        for (const auto& line : lines)
            out << line << "\n";

        out.close();
    }

    void drawCmds(size_t contentWidthTmp) {
        contentWidth = contentWidthTmp;
        size_t totalWidth = contentWidth + 8;

        size_t used = 0;

        for (const auto& c : cmds) {
            used += c.key.length() + c.desc.length() + 4;
        }

        used += (cmds.size() - 1) * 3;

        size_t freeSpace = (totalWidth > used) ? (totalWidth - used) : 0;
        size_t gap = (cmds.size() > 1) ? freeSpace / (cmds.size() - 1) : 0;
        size_t extra = (cmds.size() > 1) ? freeSpace % (cmds.size() - 1) : 0;

        printSeparator("┌", "─", "┬", "┐", gap, extra);
        
        std::cout << COLOR_BORDER << "│ ";

        for (size_t i = 0; i < cmds.size(); i++) {
            std::cout
                << COLOR_CMD << ":" << cmds[i].key
                << COLOR_BORDER << " "
                << COLOR_DESC << cmds[i].desc
                << COLOR_BORDER;

            if (i != cmds.size() - 1) {
                size_t thisGap = gap + (extra > 0 ? 1 : 0);
                if (extra > 0) extra--;

                for (size_t j = 0; j < thisGap; j++)
                    std::cout << ' ';

                std::cout << COLOR_BORDER << "│ ";
            }
        }

        size_t printedWidth = used + (cmds.size() - 1) * gap + extra;

        size_t pad = ((totalWidth + 3) > printedWidth)
            ? ((totalWidth + 3) - printedWidth)
            : 0;

        for (size_t i = 0; i < pad; i++)
            std::cout << ' ';

        std::cout << COLOR_BORDER << "│\n" << COLOR_RESET;

        printSeparator("└", "─", "┴", "┘", gap, extra);
    }

    void drawUI() {
        sys.restoreScreen();
        linesCount = minHeight;
        size_t width = 58;

        contentWidth = filename.length() + 6;

        for (const auto& line : lines)
        {
            size_t lineWidth = line.length() + 6;
            if (lineWidth > contentWidth)
                contentWidth = lineWidth;
        }

        size_t cmdWidth =
            10 + 6 +
            10 + 6 +
            8  + 6 +
            12 + 6 +
            12;

        if (cmdWidth > contentWidth)
            contentWidth = cmdWidth;

        if (contentWidth < width)
            contentWidth = width;

        std::cout << COLOR_BORDER;
        printHorizontal("┌", "─", "┐");

        std::cout << COLOR_BORDER << "│ " << COLOR_OK << filename;

        size_t padding = contentWidth - filename.length() - 2;

        for (size_t i = 0; i < padding; i++)
            std::cout << ' ';

        std::cout << COLOR_BORDER << " │\n";

        printHorizontal("├", "─", "┤");

        printHorizontal("│", " ", "│");

        for (size_t i = 0; i < lines.size(); i++) {
            size_t offset = 1;
            std::cout << COLOR_BORDER << "│" << COLOR_RESET;

            std::cout << COLOR_INFO << std::setw(2) << i + 1 << " │ " << COLOR_LINE << lines[i];

            size_t used = lines[i].length() + 6;
            size_t pad = (contentWidth > used) ? (contentWidth - used) : 0;


            for (size_t j = 0; j < (pad + offset); j++)
                std::cout << ' ';

            std::cout << COLOR_BORDER << "│\n" << COLOR_RESET;
            linesCount++;
        }

        printHorizontal("│", " ", "│");

        //printHorizontal("├", "─", "┤");

        printHorizontal("└", "─", "┘");

        std::cout << "\n";

        renderBottomBox();
    }

    void run() {
        std::string command;

        sys.clearScreen();

        while (true) {
            drawUI();

            std::cout << COLOR_OK << "vim> " << COLOR_RESET;
            std::getline(std::cin, command);

            if (command == ":q") {
                break;
            }

            else if (command == ":w") {
                saveFile();

                std::cout
                    << COLOR_OK
                    << "File saved.     |";

                system("pause");
            }

            else if (command == ":a") {
                std::string newLine;

                std::cout
                    << COLOR_CMD
                    << "New Line: ";

                std::getline(std::cin, newLine);

                lines.push_back(newLine);
                saveFile();
            }

            else if (command.rfind(":e ", 0) == 0) {
                try {
                    int lineNum = std::stoi(command.substr(3)) - 1;

                    if (lineNum >= 0 &&
                        lineNum < static_cast<int>(lines.size())) {

                        std::string newLine;

                        std::cout
                            << COLOR_CMD
                            << "Edit Line "
                            << lineNum + 1
                            << ": ";

                        std::getline(std::cin, newLine);

                        lines[lineNum] = newLine;
                    }
                    saveFile();
                }
                catch (...) {
                    std::cout
                        << COLOR_ERROR
                        << "Invalid line number.    |";

                    system("pause");
                }
            }

            else if (command.rfind(":d ", 0) == 0) {
                try {
                    int lineNum = std::stoi(command.substr(3)) - 1;

                    if (lineNum >= 0 &&
                        lineNum < static_cast<int>(lines.size())) {

                        lines.erase(lines.begin() + lineNum);
                    }
                    saveFile();
                }
                catch (...) {
                    std::cout
                        << COLOR_ERROR
                        << "Invalid line number.    |";

                    system("pause");
                }
            }

            else {
                std::cout
                    << COLOR_ERROR
                    << "Unknown command.    |";

                system("pause");
            }
        }

        saveFile();

        std::cout
            << COLOR_OK
            << "Saved: "
            << filePath
            << COLOR_RESET
            << std::endl;
    }
};

void cmd_vim(const std::vector<std::string>& args) {
    if (args.size() < 2) {
        help.printHelp("vim", {"file"}, false, "", true);

        return;
    }

    Vim editor(args[1]);
    editor.run();
}


// sudo required: Passwort ändern
void cmd_passwd(const std::vector<std::string>& args, Terminal& term) {
    (void)args;
    User* user = term.getCurrentUser();
    std::string oldPass;
    std::cout << currentColor + "Current \033[0;33mPassword\033[0m: ";
    oldPass = getHiddenInput();

    if (oldPass != user->password) {
        std::cout << currentColor + "Authentication failed.\n";
        return;
    }

    std::string newPass1, newPass2;
    std::cout << currentColor + "Enter new \033[0;33mPassword\033[0m: ";
    newPass1 = getHiddenInput();
    std::cout << currentColor + "Retype new \033[0;33mPassword\033[0m: ";
    newPass2 = getHiddenInput();

    if (newPass1 != newPass2) {
        std::cout << currentColor + "\033[0;33mPasswords\033[0m do not match.\n";
        return;
    }

    user->password = newPass1;
    std::cout << currentColor + "\033[0;33mPassword\033[0m updated successfully.\n";
}

// Funktion zum Ändern eines einzelnen Feldes
bool updateUserData(std::unordered_map<std::string, User>& users,
                     const std::string& username,
                     const std::string& field,
                     const std::string& newValue)
{
    auto it = users.find(username);
    if (it == users.end()) {
        return false; // Benutzer nicht gefunden
    }

    User& user = it->second;

    if (field == "userid") user.userId = stoi(newValue);
    else if (field == "preName") user.preName = newValue;
    else if (field == "lastName") user.lastName = newValue;
    else if (field == "name") user.name = newValue;
    else if (field == "password") user.password = newValue;
    else if (field == "created") user.created = newValue;
    else if (field == "rank") user.rank = newValue;
    else if (field == "userRights") user.userRights = newValue;
    else return false; // Feld existiert nicht

    return true;
}

void cmd_perm(const std::vector<std::string>& args, Terminal& term, std::string error = "none") {
    if (args.size() < 2) {
        help.printHelp("perm", {"list", "edit", "info", "help"}, true, "", true);
        return;
    }

    if (error != "none") {
        User* currentUser = term.getCurrentUser();
        std::string prompt = "\033[1;32m" + currentUser->name + "@GeistOS\033[0m:\033[0;34m" + currentDictonary + "\033[0m$";
        std::cout << currentColor + prompt << "sudo perm (list/edit/info/help)\n";
        std::cout << currentColor + error << "\n";
    }

    if (args[1] == "list") {
        auto& users = term.getUsers();
        std::cout << currentColor + "All Users Permissions:\n";

        VariadicTable<int, std::string, std::string, std::string> userTable({"UserId", "Username", "Rights", "Permissions"});

        // 1) Kopiere Paare in einen Vektor (Erhält die aktuelle Iterations-Reihenfolge)
        std::vector<std::pair<const std::string, User>> vec;
        vec.reserve(users.size());
        for (const auto& p : users) {
            vec.push_back(p);
        }

        // 2) Iteriere den Vektor rückwärts
        for (auto it = vec.rbegin(); it != vec.rend(); ++it) {
            const User& u = it->second;
            std::string userPermissions = "";

            std::string allRights = "rwxs";
            for (int i = 0; i < 4; i++) {
                if (u.userRights[i] == '1') {
                    userPermissions += allRights[i];
                } else {
                    userPermissions += '-';
                }
            }

            userTable.addRow({u.userId, u.name, u.userRights, userPermissions});
        }

        userTable.print(std::cout << currentColor);



    } else if (args[1] == "edit") {
        std::cout << currentColor + "All Available Rights: \n\n";
        VariadicTable<std::string, std::string, std::string, std::string> permTable({"Octal", "Decimal", "Permission", "Representation"});

        permTable.addRow({"0000", "0  (0 + 0 + 0 + 0)", "No Permission", "----"});
        permTable.addRow({"0001", "1  (0 + 0 + 0 + 1)", "Sudo", "---s"});
        permTable.addRow({"0010", "2  (0 + 0 + 2 + 0)", "Execute", "--x-"});
        permTable.addRow({"0011", "3  (0 + 0 + 2 + 1)", "Execute + Sudo", "--xs"});

        permTable.addRow({"0100", "4  (0 + 4 + 0 + 0)", "Write", "-w--"});
        permTable.addRow({"0101", "5  (0 + 4 + 0 + 1)", "Write + Sudo", "-w-s"});
        permTable.addRow({"0110", "6  (0 + 4 + 2 + 0)", "Write + Execute", "-wx-"});
        permTable.addRow({"0111", "7  (0 + 4 + 2 + 1)", "Write + Execute + Sudo", "-wxs"});

        permTable.addRow({"1000", "8  (8 + 0 + 0 + 0)", "Read", "r---"});
        permTable.addRow({"1001", "9  (8 + 0 + 0 + 1)", "Read + Sudo", "r--s"});
        permTable.addRow({"1010", "10 (8 + 0 + 2 + 0)", "Read + Execute", "r-x-"});
        permTable.addRow({"1011", "11 (8 + 0 + 2 + 1)", "Read + Execute + Sudo", "r-xs"});

        permTable.addRow({"1100", "12 (8 + 4 + 0 + 0)", "Read + Write", "rw--"});
        permTable.addRow({"1101", "13 (8 + 4 + 0 + 1)", "Read + Write + Sudo", "rw-s"});
        permTable.addRow({"1110", "14 (8 + 4 + 2 + 0)", "Read + Write + Execute", "rwx-"});
        permTable.addRow({"1111", "15 (8 + 4 + 2 + 1)", "Read + Write + Execute + Sudo", "rwxs"});

        permTable.print(std::cout << currentColor);

        std::cout << currentColor + "\n\n";

        auto& users = term.getUsers();
        std::cout << currentColor + "All Users Permissions:\n";

        VariadicTable<int, std::string, std::string, std::string> userTable({"UserId", "Username", "Rights", "Permissions"});

        // 1) Kopiere Paare in einen Vektor (Erhält die aktuelle Iterations-Reihenfolge)
        std::vector<std::pair<const std::string, User>> vec;
        vec.reserve(users.size());
        for (const auto& p : users) {
            vec.push_back(p);
        }

        // 2) Iteriere den Vektor rückwärts
        for (auto it = vec.rbegin(); it != vec.rend(); ++it) {
            const User& u = it->second;
            std::string userPermissions = "";

            std::string allRights = "rwxs";
            for (int i = 0; i < 4; i++) {
                if (u.userRights[i] == '1') {
                    userPermissions += allRights[i];
                } else {
                    userPermissions += '-';
                }
            }

            userTable.addRow({u.userId, u.name, u.userRights, userPermissions});
        }

        userTable.print(std::cout << currentColor);
        std::string userName;
        std::cout << currentColor + "\n" << "What \033[1;32mUsers\033[0m \033[0;33mPermission\033[0m do you want to change?: ";
        std::getline(std::cin, userName);

        if (users.find(userName) == users.end()) {
            std::cout << currentColor + "\033[1;32mUser\033[0m not found.\n";
            return;
        }

        std::string userRightsNew;
        std::cout << currentColor + "\n" << "What should be the \033[1;32mUsers\033[0m new \033[0;33mRights\033[0m?: ";
        std::getline(std::cin, userRightsNew);

        std::string userPermissions = "";
        std::string allRights = "rwxs";
        for (int i = 0; i < 4; i++) {
            if (userRightsNew[i] == '1') {
                userPermissions += allRights[i];
            } else {
                userPermissions += '-';
            }
        }

        // Feld ändern
        if (updateUserData(users, userName, "userRights", userRightsNew)) {
            std::cout << "Rights changed successfully!\n" << "New Rights of \033[1;32m" + userName + "\033[0m: \033[0;33m" + userRightsNew + "\033[0m\nSo the \033[1;32mUser\033[0m can: \033[0;33m" + userPermissions + "\033[0m\n";
        } else {
            std::cout << "Error while changing the \033[1;32mUsers\033[0m Rights.\n";
        }



    } else if (args[1] == "info") {
        VariadicTable<std::string, std::string, std::string, std::string> permTable({"Octal", "Decimal", "Permission", "Representation"});

        permTable.addRow({"0000", "0  (0 + 0 + 0 + 0)", "No Permission", "----"});
        permTable.addRow({"0001", "1  (0 + 0 + 0 + 1)", "Sudo", "---s"});
        permTable.addRow({"0010", "2  (0 + 0 + 2 + 0)", "Execute", "--x-"});
        permTable.addRow({"0011", "3  (0 + 0 + 2 + 1)", "Execute + Sudo", "--xs"});

        permTable.addRow({"0100", "4  (0 + 4 + 0 + 0)", "Write", "-w--"});
        permTable.addRow({"0101", "5  (0 + 4 + 0 + 1)", "Write + Sudo", "-w-s"});
        permTable.addRow({"0110", "6  (0 + 4 + 2 + 0)", "Write + Execute", "-wx-"});
        permTable.addRow({"0111", "7  (0 + 4 + 2 + 1)", "Write + Execute + Sudo", "-wxs"});

        permTable.addRow({"1000", "8  (8 + 0 + 0 + 0)", "Read", "r---"});
        permTable.addRow({"1001", "9  (8 + 0 + 0 + 1)", "Read + Sudo", "r--s"});
        permTable.addRow({"1010", "10 (8 + 0 + 2 + 0)", "Read + Execute", "r-x-"});
        permTable.addRow({"1011", "11 (8 + 0 + 2 + 1)", "Read + Execute + Sudo", "r-xs"});

        permTable.addRow({"1100", "12 (8 + 4 + 0 + 0)", "Read + Write", "rw--"});
        permTable.addRow({"1101", "13 (8 + 4 + 0 + 1)", "Read + Write + Sudo", "rw-s"});
        permTable.addRow({"1110", "14 (8 + 4 + 2 + 0)", "Read + Write + Execute", "rwx-"});
        permTable.addRow({"1111", "15 (8 + 4 + 2 + 1)", "Read + Write + Execute + Sudo", "rwxs"});

        permTable.print(std::cout << currentColor);



    } else if (args[1] == "help") {
        std::cout << currentColor + "The Permission command: \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34mperm list\033[0m \033[0;33m(Show the Currents Permissions of the Users)\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34mperm edit\033[0m \033[0;33m()\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34mperm info\033[0m \033[0;33m(Show the Permission Table)\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34mperm help\033[0m \033[0;33m(Show this Help)\033[0m \n";
        


    } else {
        help.printHelp("perm", {"list", "edit", "info", "help"}, true, "", true);
    }
}

struct EditOption {
    std::string displayName;
    std::string dbName;
};

std::vector<EditOption> editOptions = {
    {"UserId", "userid"},
    {"UserName", "name"},
    {"PreName", "preName"},
    {"LastName", "lastName"},
    {"Password", "password"},
    {"Created", "created"},
    {"Rank", "rank"},
    {"Rights", "userRights"}
};

std::vector<EditOption> rankOptions = {
    {"Member", "1"},
    {"Admin", "2"},
    {"Owner", "3"},
    {"Bot", "4"},
    {"Tester", "5"}
};

void cmd_user(const std::vector<std::string>& args, Terminal& term, std::string error = "none") {
    if (args.size() < 2) {
        help.printHelp("user", {"list", "add", "edith", "del", "help"}, true, "", true);
        return;
    }

    if (args[1] == "list") {
        auto& users = term.getUsers();
        std::cout << currentColor + "All users:\n";

        VariadicTable<int, std::string, std::string, std::string, std::string, std::string, std::string, std::string, std::string> userTable({"UserId", "Username", "Prename", "Lastname", "Password", "Created", "Rank", "Rights", "Permissions"});

        std::vector<std::pair<const std::string, User>> vec;
        vec.reserve(users.size());
        for (const auto& p : users) {
            vec.push_back(p);
        }

        std::string allRights = "rwxs";
        for (auto it = vec.rbegin(); it != vec.rend(); ++it) {
            const User& u = it->second;
            std::string userPermissions = "";

            for (int i = 0; i < 4; i++) {
                if (u.userRights[i] == '1') {
                    userPermissions += allRights[i];
                } else {
                    userPermissions += '-';
                }
            }

            userTable.addRow({u.userId, u.name, u.preName, u.lastName, u.password, u.created, u.rank, u.userRights, userPermissions});
        }

        userTable.print(std::cout << currentColor);



    } else if (args[1] == "add") {
        if (error != "none") {
            User* currentUser = term.getCurrentUser();
            std::string prompt = "\033[1;32m" + currentUser->name + "@GeistOS\033[0m:\033[0;34m" + currentDictonary + "\033[0m$";
            std::cout << currentColor + prompt << "sudo user add\n";
            std::cout << currentColor + error << "\n";
        }

        std::string userName;
        std::cout << currentColor + "What is the \033[0;33mUsername\033[0m?: ";
        std::getline(std::cin, userName);

        auto& users = term.getUsers();
        if (users.find(userName) != users.end()) {
            std::cout << currentColor + userName << " already exists.\n";
            return;
        }

        std::string preName;
        std::cout << currentColor + "What is \033[1;32m" << userName << "s\033[0m \033[0;33mPrename\033[0m?: ";
        std::getline(std::cin, preName);

        std::string lastName;
        std::cout << currentColor + "What is \033[1;32m" << userName << "s\033[0m \033[0;33mLastname\033[0m?: ";
        std::getline(std::cin, lastName);

        std::string pass;
        std::cout << currentColor + "Enter \033[0;33mPassword\033[0m for \033[1;32m" << userName << "\033[0m: ";
        pass = getHiddenInput();

        std::string created;
        std::cout << currentColor + "When should \033[1;32m" << userName << "\033[0m be \033[0;33mCreated\033[0m?: ";
        std::getline(std::cin, created);

        std::string userRank;
        std::string userRankIndex;

        std::cout << currentColor + "Available \033[0;33mRanks\033[0m: \n";

        // Menü dynamisch generieren
        for (size_t i = 0; i < rankOptions.size(); ++i) {
            std::cout << currentColor << "   " << (i + 1)
                    << ". \033[1;36m" << rankOptions[i].displayName << "\033[0m\n";
        }

        std::cout << currentColor + "What should \033[1;32m" << userName << "s\033[0m \033[0;33mRank\033[0m be? (1 - " << std::to_string(rankOptions.size()) << "): ";
        std::getline(std::cin, userRankIndex);

        // Eingabe prüfen
        int choice = std::stoi(userRankIndex);
        if (choice < 1 || choice > std::stoi(std::to_string(rankOptions.size()))) {
            std::string errorMsg = "\033[0;31mPlease only enter the Numbers 1 - " + std::to_string(rankOptions.size()) + "!\033[0m";
            cmd_user(args, term, errorMsg);
            return;
        }
        userRank = rankOptions[choice - 1].displayName;

        std::string userRights;
        std::cout << currentColor + "What \033[0;33mRights\033[0m should \033[1;32m" << userName << "\033[0m have?: ";
        std::getline(std::cin, userRights);

        std::cout << currentColor + "\n" << "User \033[1;32m" << userName << "\033[0m added successfully!" << "\n";

        VariadicTable<int, std::string, std::string, std::string, std::string, std::string, std::string, std::string> userTable({"UserId", "Prename", "Lastname", "Username", "Password", "Created", "Rank", "Rights"});

        userTable.addRow({(int) users.size(), preName, lastName, userName, pass, created, userRank, userRights});
        //users[userName] = {(int) users.size(), preName, lastName, userName, pass, created, userRank, userRights};
        users.emplace(userName, User{
            static_cast<int>(users.size()),
            preName,
            lastName,
            userName,
            pass,
            created,
            userRank,
            userRights
        });
        userTable.print(std::cout << currentColor);



    } else if (args[1] == "edit") {
        if (error != "none") {
            User* currentUser = term.getCurrentUser();
            std::string prompt = "\033[1;32m" + currentUser->name + "@GeistOS\033[0m:\033[0;34m" + currentDictonary + "\033[0m$";
            std::cout << currentColor + prompt << "sudo user edit\n";
            std::cout << currentColor + error << "\n";
        }

        auto& users = term.getUsers();
        std::cout << currentColor + "All users:\n";

        VariadicTable<int, std::string, std::string, std::string, std::string, std::string, std::string, std::string, std::string> userTable({"UserId", "Username", "Prename", "Lastname", "Password", "Created", "Rank", "Rights", "Permissions"});

        // 1) Kopiere Paare in einen Vektor (Erhält die aktuelle Iterations-Reihenfolge)
        std::vector<std::pair<const std::string, User>> vec;
        vec.reserve(users.size());
        for (const auto& p : users) {
            vec.push_back(p);
        }

        // 2) Iteriere den Vektor rückwärts
        std::string allRights = "rwxs";
        for (auto it = vec.rbegin(); it != vec.rend(); ++it) {
            const User& u = it->second;
            std::string userPermissions = "";

            for (int i = 0; i < 4; i++) {
                if (u.userRights[i] == '1') {
                    userPermissions += allRights[i];
                } else {
                    userPermissions += '-';
                }
            }

            userTable.addRow({u.userId, u.name, u.preName, u.lastName, u.password, u.created, u.rank, u.userRights, userPermissions});
        }

        userTable.print(std::cout << currentColor);

        std::string userName;
        std::cout << currentColor + "What \033[1;32mUser\033[0m do you want to \033[0;33medit\033[0m? (Enter \033[1;32mUsername\033[0m): ";
        std::getline(std::cin, userName);

        if (users.find(userName) == users.end()) {
            std::string errorMsg = "\033[1;32mUser\033[0m not found.\n";
            cmd_user(args, term, errorMsg);
            return;
        }

        std::string editNum;
        std::string editTitle;

        std::cout << currentColor + "What Data do you want to \033[0;33medit\033[0m?: \n";

        // Menü dynamisch generieren
        for (size_t i = 0; i < editOptions.size(); ++i) {
            std::cout << currentColor << "   " << (i + 1)
                    << ". \033[1;36m" << editOptions[i].displayName << "\033[0m\n";
        }

        std::cout << currentColor << "Choose one Number (1 - " << editOptions.size() << "): ";
        std::getline(std::cin, editNum);

        // Eingabe prüfen
        int choice = std::stoi(editNum);
        editTitle = editOptions[choice - 1].displayName;
        editNum = editOptions[choice - 1].dbName;

        std::string userDataNew;
        std::cout << currentColor + "What should be the \033[0;33mNew\033[0m \033[1;32m" + editTitle + "\033[0m?: ";
        std::getline(std::cin, userDataNew);

        // Feld ändern
        if (updateUserData(users, userName, editNum, userDataNew)) {
            std::cout << editTitle + " changed successfully!\n" << "New " + editTitle + " of \033[1;32m" + userName + "\033[0m: \033[0;33m" + userDataNew + "\n";
        } else {
            std::cout << "Error while changing the \033[1;32mUsers\033[0m \033[1;32m" + editTitle + "\033[0m.\n";
        }



    } else if (args[1] == "del") {
        std::string userName;
        std::cout << currentColor + "What \033[1;32mUser\033[0m should be \033[0;31mdeleted\033[0m?: ";
        std::getline(std::cin, userName);

        auto& users = term.getUsers();
        if (users.find(userName) == users.end()) {
            std::cout << currentColor + "\033[1;32mUser\033[0m not found.\n";
            return;
        }

        users.erase(userName);
        std::cout << currentColor + "User \033[1;32m" << userName << "\033[0m \033[0;31mdeleted\033[0m.\n";



    } else if (args[1] == "help") {
        std::cout << currentColor + "The user command: \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34muser list\033[0m \033[0;33m(Lists all \033[1;32mUsers\033[0;33m)\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34muser add\033[0m \033[0;33m(Add a new \033[1;32mUser\033[0;33m)\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34muser del\033[0m \033[0;33m(delete a certain \033[1;32mUser\033[0;33m)\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34muser edit\033[0m \033[0;33m(Change certain Data of a certain \033[1;32mUser\033[0;33m)\033[0m \n";
        std::cout << currentColor + "   - \033[1;36msudo\033[0m \033[1;34muser help\033[0m \033[0;33m(Show this Help)\033[0m \n";
        
    } else {
        help.printHelp("user", {"list", "add", "edith", "del", "help"}, true, "", true);
    }
}

void cmd_print(const std::vector<std::string>& args, Terminal& term) {
    (void)term;
    printScreen(args[1]);
}

// ---------------- TIMER CALLBACK ----------------
void UpdateGraph(Graph* g) {
    static float t = 0;
    t += 0.1f;
    g->AddValue(50 + 50 * sin(t));
}

void cmd_win(const std::vector<std::string>& args, Terminal& term) {
    (void)args; (void)term;

    Window win("Main Window", 900, 600, "MainWin");
    WindowConfig cfg;

    // -------- TOP PANEL --------
    PanelConfig top;
    top.dock = Dock::Top; 
    top.size = 50; 
    top.orientation = Orientation::Horizontal;
    top.items.push_back(new ButtonConfig("System", [&win]{ win.ShowLeftPanel("left_system"); }));
    top.items.push_back(new ButtonConfig("Apps", [&win]{ win.ShowLeftPanel("left_apps"); }));

    // -------- LEFT PANEL SYSTEM --------
    PanelConfig leftSys;
    leftSys.dock = Dock::Left; 
    leftSys.size = 200; 
    leftSys.id = "left_system"; 
    leftSys.orientation = Orientation::Vertical;
    leftSys.defaultMainPanelId = "main_sys_gen";
    leftSys.items.push_back(new ButtonConfig("General", [&win]{ win.ShowMainPanel("main_sys_gen"); }));
    leftSys.items.push_back(new ButtonConfig("Test", [&win]{ win.ShowMainPanel("main_sys_table"); }));

    // -------- LEFT PANEL APPS --------
    PanelConfig leftApps;
    leftApps.dock = Dock::Left; 
    leftApps.size = 200; 
    leftApps.id = "left_apps"; 
    leftApps.orientation = Orientation::Vertical;
    leftApps.defaultMainPanelId = "main_apps";
    leftApps.items.push_back(new ButtonConfig("Installed", [&win]{ win.ShowMainPanel("main_apps"); }));
    leftApps.items.push_back(new ButtonConfig("Available", [&win]{ win.ShowMainPanel("available_apps"); }));

    // -------- MAIN PANELS --------
    PanelConfig GenPan; 
    GenPan.id = "main_sys_gen"; 
    GenPan.orientation = Orientation::Vertical;
    GenPan.items.push_back(new TextConfig("System > General"));

    GenPan.items.push_back(new TextConfig("Graph Example"));
    GenPan.items.push_back(new GraphConfig(300,150, {"A","B","C","D","E"})); // Balken-Titel

    PanelConfig GenTable; 
    GenTable.id = "main_sys_table";
    GenTable.orientation = Orientation::Vertical;
    GenTable.items.push_back(new TextConfig("System > Table"));

    GenTable.items.push_back(new TextConfig("Table Example"));
    GenTable.items.push_back(new TableConfig({ "Octal","Decimal","Permission", "Representation" }));

    for (auto e : win.m_elements) {
        if (auto t = dynamic_cast<Table*>(e)) {
            std::vector<std::vector<std::string>> rows = {
                {"0000", "0 (0 + 0 + 0 + 0)", "No Permission", "----"},
                {"0001", "1 (0 + 0 + 0 + 1)", "Sudo", "---s"},
                {"0010", "2 (0 + 0 + 2 + 0)", "Execute", "--x-"},
                {"0011", "3 (0 + 0 + 2 + 1)", "Execute + Sudo", "--xs"},
                {"0100", "4 (0 + 4 + 0 + 0)", "Write", "-w--"},
                {"0101", "5 (0 + 4 + 0 + 1)", "Write + Sudo", "-w-s"},
                {"0110", "6 (0 + 4 + 2 + 0)", "Write + Execute", "-wx-"},
                {"0111", "7 (0 + 4 + 2 + 1)", "Write + Execute + Sudo", "-wxs"},
                {"1000", "8 (8 + 0 + 0 + 0)", "Read", "r---"},
                {"1001", "9 (8 + 0 + 0 + 1)", "Read + Sudo", "r--s"},
                {"1010", "10 (8 + 0 + 2 + 0)", "Read + Execute", "r-x-"},
                {"1011", "11 (8 + 0 + 2 + 1)", "Read + Execute + Sudo", "r-xs"},
                {"1100", "12 (8 + 4 + 0 + 0)", "Read + Write", "rw--"},
                {"1101", "13 (8 + 4 + 0 + 1)", "Read + Write + Sudo", "rw-s"},
                {"1110", "14 (8 + 4 + 2 + 0)", "Read + Write + Execute", "rwx-"},
                {"1111", "15 (8 + 4 + 2 + 1)", "Read + Write + Execute + Sudo", "rwxs"}
            };

            for (const auto& row : rows) {
                LVITEM item{};
                item.mask = LVIF_TEXT;
                item.iItem = ListView_GetItemCount(t->hListView);
                item.iSubItem = 0;
                item.pszText = (LPSTR)row[0].c_str(); // ANSI
                ListView_InsertItem(t->hListView, &item);

                for (size_t col = 1; col < row.size(); col++) {
                    LVITEM subitem{};
                    subitem.mask = LVIF_TEXT;
                    subitem.iItem = item.iItem;
                    subitem.iSubItem = (int)col;
                    subitem.pszText = (LPSTR)row[col].c_str(); // ANSI
                    ListView_SetItem(t->hListView, &subitem);
                }
            }
        }
    }

    PanelConfig m3; 
    m3.id = "main_apps";
    m3.items.push_back(new TextConfig("Apps > Installed"));
    
    PanelConfig m4; 
    m4.id = "available_apps"; 
    m4.items.push_back(new TextConfig("Apps > Available"));

    cfg.panels = { top, leftSys, leftApps, GenPan, GenTable, m3, m4 };
    win.LoadFromConfig(cfg);

    win.ShowLeftPanel("left_system");
    win.ShowMainPanel("main_sys_gen");

    // Finde Graph-Element
    Graph* graph = nullptr;
    for (auto e : win.m_elements) if (auto g = dynamic_cast<Graph*>(e)) { graph = g; break; }

    // Hauptloop
    while (win.ProcessMessages()) {
        if (graph) UpdateGraph(graph);
        std::this_thread::sleep_for(std::chrono::milliseconds(100));
    }

    for (auto& p : cfg.panels)
        for (auto item : p.items) delete item;
}

void cmd_date(const std::vector<std::string>& args, Terminal& term) {
    (void) args; (void) term;
    auto now = std::chrono::system_clock::now();
    std::time_t currentTime = std::chrono::system_clock::to_time_t(now);
    std::tm *ltm = std::localtime(&currentTime);

    const char* weekdays[] = {
        "Sunday","Monday","Tuesday",
        "Wednesday","Thursday","Friday","Saturday"
    };

    std::ostringstream date, time;

    date << (1900 + ltm->tm_year) << "-"
         << std::setw(2) << std::setfill('0') << (1 + ltm->tm_mon) << "-"
         << std::setw(2) << ltm->tm_mday;

    time << std::setw(2) << std::setfill('0') << ltm->tm_hour << ":"
         << std::setw(2) << ltm->tm_min << ":"
         << std::setw(2) << ltm->tm_sec;

    ConsoleWindow win(50, "Current Date");

    win.addConfig({
        {
            "General", {
                {"Date", date.str()},
                {"Time", time.str()},
                {"Day", weekdays[ltm->tm_wday]}
            }
        }, {
            "Date Details", {
                {"Year", std::to_string(1900 + ltm->tm_year)},
                {"Month", std::to_string(1 + ltm->tm_mon)},
                {"Day", std::to_string(ltm->tm_mday)},
                {"Day of Year", std::to_string(ltm->tm_yday)}
            }
        }, {
            "Time Details", {
                {"Hour", std::to_string(ltm->tm_hour)},
                {"Minute", std::to_string(ltm->tm_min)},
                {"Second", std::to_string(ltm->tm_sec)}
            }
        }, {
            "Other", {
                {"Summertime", ltm->tm_isdst ? "Yes" : "No"}
            }
        }
    });

    win.render();
}

void cmd_dateOld(const std::vector<std::string>& args, Terminal& term) {
    (void) args; (void) term;
    auto now = std::chrono::system_clock::now();
    std::time_t currentTime = std::chrono::system_clock::to_time_t(now);
    std::tm *ltm = std::localtime(&currentTime);

    const std::string RESET  = "\033[0m";
    const std::string TITLE  = "\033[1;37m";
    const std::string BORDER = "\033[1;36m";
    const std::string LABEL  = "\033[1;34m";
    const std::string VALUE  = "\033[1;32m";
    const std::string DIM    = "\033[2m";

    const char* weekdays[] = {
        "Sunday","Monday","Tuesday",
        "Wednesday","Thursday","Friday","Saturday"
    };

    std::ostringstream date, time;
    date << (1900 + ltm->tm_year) << "-"
         << std::setfill('0') << std::setw(2) << (1 + ltm->tm_mon) << "-"
         << std::setw(2) << ltm->tm_mday;

    time << std::setfill('0')
         << std::setw(2) << ltm->tm_hour << ":"
         << std::setw(2) << ltm->tm_min  << ":"
         << std::setw(2) << ltm->tm_sec;

    const int W = 50;

    auto line = [&](char left, char fill, char right) {
        std::cout << BORDER << left << std::string(W - 2, fill) << right << RESET << "\n";
    };

    auto row = [&](const std::string& l, const std::string& v) {
        std::ostringstream out;
        out << " " << LABEL << std::left << std::setw(18) << l << RESET
            << " " << VALUE << std::left << std::setw(27) << v << RESET << " ";
        std::string s = out.str();
        if ((int)s.size() < W - 2) s += std::string(W - 2 - s.size(), ' ');
        std::cout << BORDER << "|" << RESET << s << BORDER << "|" << RESET << "\n";
    };

    line('+','-','+');

    std::string title = " Current Date ";
    int pad = (W - 2 - (int)title.size()) / 2;
    std::cout << BORDER << "|"
              << std::string(pad, ' ')
              << TITLE << title << RESET
              << std::string(W - 2 - pad - title.size(), ' ')
              << BORDER << "|" << RESET << "\n";

    line('+','-','+');

    row("Date", date.str());
    row("Time", time.str());
    row("Day", weekdays[ltm->tm_wday]/* + std::string(" (") + std::to_string(ltm->tm_wday) + ")"*/);

    line('+','-','+');

    row("Year", std::to_string(1900 + ltm->tm_year));
    row("Month", std::to_string(1 + ltm->tm_mon));
    row("Day", std::to_string(ltm->tm_mday));
    row("Day of the Year", std::to_string(ltm->tm_yday));

    line('+','-','+');
    
    row("Hour", std::to_string(ltm->tm_hour));
    row("Minute", std::to_string(ltm->tm_min));
    row("Second", std::to_string(ltm->tm_sec));
    
    line('+','-','+');
    
    row("Summertime", ltm->tm_isdst ? "Yes" : "No");

    line('+','-','+');
    //std::cout << DIM << "Wochentag: 0=Sonntag, 6=Samstag" << RESET << "\n";
}

std::string curVersion;

class Versions: public Config {
private: 
    std::vector<versionHistory> allVersions;

    void initVersions() {
        allVersions = config.getAllVersions();
    }

    void printVersion(const std::string& version) {
        curVersion = version;

        std::cout << currentColor
                << "\n   === \033[1;37mGeistOS\033[0m "
                << "\033[1;35mV" << version << "\033[0m "
                << "===\n";
    }

    void printSubVersion(const std::string& version) {
        curVersion = version;

        std::cout << currentColor
                << "   |-- \033[1;36mV" << version << "\033[0m\n";
    }

    void printUpdate(const std::string& prefix, const std::string& color, const std::string& change) {
        std::cout << currentColor
                << "   |   |-- \033[1;30m[" << prefix << "]\033[0m "
                << color << change
                << "\033[0m\n";
    }

    void printVersionHistory() {
        for (const auto& version : allVersions) {
            printVersion(version.title);

            for (const auto& subVersion : version.subVersions) {
                printSubVersion(subVersion.title);

                for (const auto& change : subVersion.changes) {
                    printUpdate(change.type, change.color, change.description);
                }
            }
        }
    }

public:
    void render() {
        initVersions();
        printVersionHistory();
    }
};

// =========================
// Fake System States
// =========================
std::map<std::string, std::string> sysConfig = {
    {"hostname", "GeistOS-PC"},
    {"theme", "dark"},
    {"version_channel", "stable"}
};

struct Task {
    int pid;
    std::string name;
};

std::vector<Task> tasks = {
    {1, "init"},
    {2, "terminal"},
    {3, "ghost_anim"}
};

int nextPID = 4;

#define C_RESET "\033[0m"
#define C_CMD "\033[1;34m"
#define C_ERR "\033[1;31m"
#define C_OK "\033[1;32m"
#define C_WARN "\033[1;33m"
#define C_TITLE "\033[1;35m"
#define C_VAL "\033[0;36m"


void cmd_sys(const std::vector<std::string>& args, Terminal& term) {

    if (args.size() < 2) {
        help.printHelp("sys", {"info", "uptime", "time", "tasks", "run", "kill", "mem", "cpu", "config", "host", "update", "clearcache", "bench"}, false, "", true);
        return;
    } else if (args[1] == "version") {
        if (args.size() < 3) {
            help.printHelp("sys version", {"history", "cur"}, false, "", true);
            return;
        }
        if (args[2] == "history") {
            Versions versions;

            printScreen("Versions");

            std::cout << currentColor + "All \033[0;35mVersions\033[0;0m:\n";
            versions.render();
            
        } else if (args[2] == "cur") {
            std::cout << currentColor + "\033[1;30mCurrent Version\033[1;0m: \033[0;37mGeistOS v\033[1;0m\033[0;35m" + curVersion + "\033[0;0m:\n";
        } else {
            help.printHelp("sys version", {"history", "cur"}, false, "", true);
        }
    } else if (args[1] == "log") {

        if (args.size() < 3) {
            help.printHelp("sys log", {"show", "clear"}, false, "", true);
            return;
        }

        if (args[2] == "show") {
            printScreen("CMD History");
            sysLog.getCmdLog();
        } else if (args[2] == "clear") {
            sysLog.clearCmdLog();
            std::cout << C_OK << "Log cleared." << C_RESET << "\n";
        } else {
            help.printHelp("sys log", {"show", "clear"}, false, "", true);
        }
    } else if (args[1] == "info") {
        curVersion = "0.6";
        auto user = term.getCurrentUser();
        printScreen("System Info");

        std::cout << C_WARN << "OS: " << C_VAL << "GeistOS\n";
        std::cout << C_WARN << "Version: " << C_VAL << curVersion << "\n";
        std::cout << C_WARN << "Username: " << C_VAL << user->name << "\n";
        std::cout << C_WARN << "User: " << C_VAL << user->preName << " " << user->lastName << "\n";
        std::cout << C_WARN << "Hostname: " << C_VAL << sysConfig["hostname"] << C_RESET << "\n";
    } else if (args[1] == "uptime") {
        auto now = std::chrono::steady_clock::now();
        auto duration = std::chrono::duration_cast<std::chrono::seconds>(now - systemStart);

        std::cout << C_WARN << "Uptime: "
                << C_VAL
                << duration.count() / 3600 << "h "
                << (duration.count() % 3600) / 60 << "m "
                << duration.count() % 60 << "s"
                << C_RESET << "\n";
    } else if (args[1] == "time") {
        std::time_t t = std::time(nullptr);
        std::cout << C_WARN << "Current Time: " << C_VAL
                << std::ctime(&t) << C_RESET;
    } else if (args[1] == "tasks") {
        printScreen("Running Tasks");

        for (auto& t : tasks) {
            std::cout << C_WARN << "PID: " << C_VAL << t.pid
                    << C_WARN << " | " << C_VAL << t.name << "\n";
        }
    } else if (args[1] == "kill") {
        if (args.size() < 3) {
            help.printHelp("sys kill", {"pid"}, false, "", true);
            return;
        }

        int pid = std::stoi(args[2]);

        tasks.erase(
            std::remove_if(tasks.begin(), tasks.end(),
                [&](Task& t){ return t.pid == pid; }),
            tasks.end()
        );

        std::cout << C_OK << "Process " << pid << " killed." << C_RESET << "\n";
    } else if (args[1] == "run") {
        if (args.size() < 3) {
            help.printHelp("sys run", {"name"}, false, "", true);
            return;
        }

        tasks.push_back({nextPID++, args[2]});
        std::cout << C_OK << "Started task: " << C_VAL << args[2] << C_RESET << "\n";
    } else if (args[1] == "mem") {
        //Simulated Data (Not real)
        int used = 512 + (rand() % 1024);
        std::cout << C_WARN << "Memory: " << C_VAL << used << "MB / 2048MB" << C_RESET << "\n";
    } else if (args[1] == "cpu") {
        //Simulated Data (Not real)
        std::cout << C_WARN << "CPU Usage: " << C_VAL << (rand() % 100) << "%" << C_RESET << "\n";
    } else if (args[1] == "config") {
        if (args.size() == 2) {
            printScreen("System Config");

            for (auto& c : sysConfig) {
                std::cout 
                    << C_WARN << std::setw(15) << std::left << c.first
                    << C_RESET << " : "
                    << C_VAL << c.second 
                    << C_RESET << "\n";
            }
        } else if (args.size() >= 5 && args[2] == "set") {

            std::string key = args[3];
            std::string value = args[4];

            if (sysConfig.find(key) == sysConfig.end()) {
                std::cout 
                    << C_ERR << "Unknown config key: "
                    << C_CMD << key 
                    << C_RESET << "\n";
                return;
            }

            std::string oldValue = sysConfig[key];
            sysConfig[key] = value;

            std::cout 
                << C_OK << "Updated "
                << C_CMD << key 
                << C_RESET << " ("
                << C_VAL << oldValue 
                << C_RESET << " -> "
                << C_VAL << value 
                << C_RESET << ")\n";
        } else if (args.size() == 4 && args[2] == "get") {
            std::string key = args[3];

            if (sysConfig.find(key) != sysConfig.end()) {
                std::cout 
                    << C_WARN << key << " = "
                    << C_VAL << sysConfig[key]
                    << C_RESET << "\n";
            } else {
                std::cout << C_ERR << "Key not found" << C_RESET << "\n";
            }
        } else {
            std::cout 
                << C_ERR << "Invalid usage.\n"
                << C_RESET;

            help.printHelp("sys config", {"set <key> <value>", "get <key>"}, false, "", true);

            std::cout 
                << C_TITLE << "\nAvailable keys:\n" 
                << C_RESET;

            for (auto& c : sysConfig) {
                std::cout << "  " << C_CMD << c.first << C_RESET << "\n";
            }
        }
    } else if (args[1] == "host") {
        if (args.size() < 3) {
            std::cout << C_WARN << "Hostname"<< C_RESET << ": " << C_VAL << sysConfig["hostname"] << C_RESET << "\n";
        } else {
            sysConfig["hostname"] = args[2];
            std::cout << C_WARN << "Hostname" << C_RESET << " changed to " << C_VAL << args[2] << C_RESET << "\n";
        }
    } else if (args[1] == "update") {
        std::cout << C_WARN << "Checking for updates...\n";
        std::this_thread::sleep_for(std::chrono::seconds(1));

        std::cout << C_WARN << "Fetching Packages ...\n";
        std::this_thread::sleep_for(std::chrono::seconds(1));

        std::cout << C_WARN << "Installing Packages ...\n";
        std::this_thread::sleep_for(std::chrono::seconds(1));

        std::cout << C_OK << "System is up to date ✔" << C_RESET << "\n";
    } else if (args[1] == "clearcache") {
        std::cout << C_WARN << "Clearing cache..." << C_RESET << "\n";
        std::this_thread::sleep_for(std::chrono::seconds(1));
        std::cout << C_OK << "Done." << C_RESET << "\n";
    } else if (args[1] == "bench") {
        printScreen("Benchmark");

        int limit = 100000000;

        if (args.size() >= 3) {
            try {
                limit = std::stoi(args[2]);
            } catch (...) {
                std::cout << C_ERR << "Invalid Number" << C_RESET << ": "
                        << C_CMD << args[2]
                        << C_RESET << "\n";
                std::cout << C_ERR << "Only enter Numbers (" << C_VAL << "1 - " << INT_MAX << C_ERR << ")" << C_RESET "\n";
                return;
            }
        }

        std::cout << C_WARN << "Running Benchmark\n" << "Iterations" << C_RESET << ": " << C_VAL << limit << C_RESET << "\n";

        auto start = std::chrono::high_resolution_clock::now();

        volatile int x = 0;
        for (int i = 0; i < limit; i++) x++;

        auto end = std::chrono::high_resolution_clock::now();
        auto dur = std::chrono::duration_cast<std::chrono::milliseconds>(end - start);

        std::cout << C_WARN << "Benchmark Time: "
                << C_VAL << dur.count() << " ms"
                << C_RESET << "\n";
        
        std::cout << C_WARN << "Benchmark Time: "
                << C_VAL << (((double) dur.count()) / 1000) << " s"
                << C_RESET << "\n";
    } else if (args[1] == "loading") {
        printScreen("Loader");

        std::string loading = "\u2588"; //█
        std::string progressBar = "";

        SetConsoleOutputCP(CP_UTF8);

        for (int i = 0; i <= 100; i+=10) {
            //std::cout << currentColor + "\rProgress: " << i << "%";
            progressBar += loading;
            std::cout << currentColor + "\rProgress: " << progressBar << " ";

            std::cout.flush();
            wait(randomNum(1000, 3000));
        }
        std::cout << std::endl;

    } else if (args[1] == "help") {
        printScreen("sys Commands");

        std::cout
        << C_CMD  << "info        " << C_RESET << "- " << C_VAL << "system info\n"
        << C_CMD  << "uptime      " << C_RESET << "- " << C_VAL << "show uptime\n"
        << C_CMD  << "time        " << C_RESET << "- " << C_VAL << "current time\n"
        << C_CMD  << "tasks       " << C_RESET << "- " << C_VAL << "list processes\n"
        << C_CMD  << "run <name>  " << C_RESET << "- " << C_VAL << "start task\n"
        << C_CMD  << "kill <pid>  " << C_RESET << "- " << C_VAL << "kill task\n"
        << C_CMD  << "cpu         " << C_RESET << "- " << C_VAL << "cpu usage\n"
        << C_CMD  << "mem         " << C_RESET << "- " << C_VAL << "memory usage\n"
        << C_CMD  << "config      " << C_RESET << "- " << C_VAL << "show config\n"
        << C_CMD  << "host        " << C_RESET << "- " << C_VAL << "show/set hostname\n"
        << C_CMD  << "update      " << C_RESET << "- " << C_VAL << "system update\n"
        << C_CMD  << "clearcache  " << C_RESET << "- " << C_VAL << "clear cache\n"
        << C_CMD  << "bench       " << C_RESET << "- " << C_VAL << "performance test\n";
    } else {
        help.printHelp("sys", {"info", "uptime", "time", "tasks", "run", "kill", "mem", "cpu", "config", "host", "update", "clearcache", "bench"}, false, "", true);
    }
}





// Farben
#define RED     "\033[31m"
#define GREEN   "\033[32m"
#define YELLOW  "\033[33m"
#define RESET   "\033[0m"

#define REDR     "\033[31m"
#define GREENR   "\033[32m"
#define YELLOWR  "\033[33m"
#define WHITER   "\033[37m"
#define RESETR   "\033[0m"

class IGame {
public:
    virtual ~IGame() = default;
    virtual std::string getName() const = 0;
    virtual void play() = 0;
};

// ---------------- BANK ----------------
class Bank {
private:
    int balance;
    std::string depositString = "Deposit";
    std::string widthdrawString = "Withdraw";

    struct Transaction {
        int id;
        std::string type;
        std::string operation;
        int amount;
        std::string timestamp;
        int balanceBefore;
        int balanceAfter;
    };

    std::vector<Transaction> history;
    int nextId = 1;

    static std::string getTimeStamp() {
        auto now = std::chrono::system_clock::now();
        std::time_t t = std::chrono::system_clock::to_time_t(now);

        std::tm tm = *std::localtime(&t);

        std::ostringstream oss;
        oss << std::put_time(&tm, "%d.%m.%Y %H:%M:%S");
        return oss.str();
    }

    static std::string formatBalance(long long value) {
        std::string s = std::to_string(value);
        std::string result;

        int count = 0;

        for (int i = (int)s.size() - 1; i >= 0; i--) {
            result.insert(result.begin(), s[i]);
            count++;

            if (count % 3 == 0 && i != 0) {
                result.insert(result.begin(), '.');
            }
        }

        return result;
    }

    void addTransaction(const std::string& type, int amount) {
        std::string operation;
        
        int balanceBefore = balance;

        if (type == widthdrawString) {
            operation = REDR + std::string("-");
            balanceBefore += amount;
        }
        else if (type == depositString) {
            operation = GREENR + std::string("+");
            balanceBefore -= amount;
        }

        Transaction t;
        t.id = nextId++;
        t.type = type;
        t.operation = operation;
        t.amount = amount;
        t.timestamp = getTimeStamp();
        t.balanceBefore = balanceBefore;
        t.balanceAfter = balance;

        history.push_back(t);
    }

public:
    Bank(int start) : balance(start) {}

    int getBalance() const {
        return balance;
    }

    bool withdraw(int amount) {
        if (amount > balance) return false;

        balance -= amount;
        addTransaction(widthdrawString, amount);
        return true;
    }

    void deposit(int amount) {
        balance += amount;
        addTransaction(depositString, amount);
    }

    void print() const {
        std::cout << YELLOWR
                  << "Balance: "
                  << formatBalance(balance)
                  << " Chips\n"
                  << RESETR;
    }

    void printHistory() const {
        for (const auto& t : history) {
            std::cout << std::left
                    << RESETR << std::setw(4)  << ("#" + std::to_string(t.id))
                    << RESETR << std::setw(22) << ("[" + t.timestamp + "]")
                    << RESETR << std::setw(9) << t.type
                    << RESETR << " | "
                    << RESETR << std::setw(20) << (t.operation + formatBalance(t.amount))
                    << RESETR << " | "
                    << RESETR << std::setw(20) << ("Old Balance: " + formatBalance(t.balanceBefore))
                    << RESETR << " | "
                    << RESETR << std::setw(20) << ("New Balance: " + formatBalance(t.balanceAfter))
                    << RESETR << "\n";
        }
    }
};






// ---------------- ROULETTE ----------------
class Roulette : public IGame {
private:
    Bank& bank;

    bool isRed(int n) {
        int red[] = {
            1,3,5,7,9,12,14,16,18,19,
            21,23,25,27,30,32,34,36
        };
        for (int r : red) if (r == n) return true;
        return false;
    }

    void printNumber(int n) {
        if (n == 0) {
            std::cout << GREENR << n << RESETR;
        } else if (isRed(n)) {
            std::cout << REDR << n << RESETR;
        } else {
            std::cout << WHITER << n << RESETR;
        }
    }

    void drawTable() {
        std::cout << "\n";
        std::cout << GREENR << "========== ROULETTE ==========\n" << RESETR;
        std::cout << "Numbers:\n";

        for (int i = 0; i <= 36; i++) {
            printNumber(i);
            std::cout << " ";

            if (i % 12 == 0 && i != 0) std::cout << "\n";
        }

        std::cout << "\n==============================\n";
    }

    void spinWheel(int finalNumber) {
        std::vector<int> wheel = {
            0,32,15,19,4,21,2,25,17,34,
            6,27,13,36,11,30,8,23,10,5,
            24,16,33,1,20,14,31,9,22,18,
            29,7,28,12,35,3,26
        };

        auto printNum = [&](int n) {
            if (n == 0) std::cout << "\033[32m" << n << "\033[0m";
            else if (isRed(n)) std::cout << "\033[31m" << n << "\033[0m";
            else std::cout << "\033[37m" << n << "\033[0m";
        };

        int index = 0;
        (void) index;

        for (int i = 0; i < (int)wheel.size(); i++) {
            if (wheel[i] == finalNumber) {
                index = i;
                break;
            }
        }

        int current = 0;
        int steps = 80 + (std::rand() % 40);

        for (int i = 0; i < steps; i++) {
            current = (current + 1) % wheel.size();

            std::cout << "\r";

            std::cout << "        ";
            for (int j = -3; j <= 3; j++) {
                int idx = (current + j + wheel.size()) % wheel.size();

                if (j == 0) {
                    std::cout << "[";
                    printNum(wheel[idx]);
                    std::cout << "]";
                } else {
                    std::cout << " ";
                    printNum(wheel[idx]);
                    std::cout << " ";
                }
            }

            std::cout << "   " << std::flush;

            std::this_thread::sleep_for(
                std::chrono::milliseconds(40 + i * 4)
            );
        }

        while (wheel[current] != finalNumber) {
            current = (current + 1) % wheel.size();

            std::cout << "\r        ";
            for (int j = -3; j <= 3; j++) {
                int idx = (current + j + wheel.size()) % wheel.size();

                if (j == 0) {
                    std::cout << "[";
                    printNum(wheel[idx]);
                    std::cout << "]";
                } else {
                    std::cout << " ";
                    printNum(wheel[idx]);
                    std::cout << " ";
                }
            }

            std::cout << "   " << std::flush;

            std::this_thread::sleep_for(
                std::chrono::milliseconds(120)
            );
        }

        std::cout << "\n";
    }

public:
    Roulette(Bank& b) : bank(b) {}

    std::string getName() const override {
        return "Roulette";
    }

    void play() override {
        int betNumber, betAmount;

        drawTable();
        bank.print();

        std::cout << "Bet on Number (0-36): ";
        std::cin >> betNumber;

        if (betNumber < 0 || betNumber > 36) {
            std::cout << REDR << "Invalid Number\n" << RESETR;
            return;
        }

        std::cout << "Bet: ";
        std::cin >> betAmount;

        if (!bank.withdraw(betAmount)) {
            std::cout << REDR << "Balance is too low\n" << RESETR;
            return;
        }

        int result = std::rand() % 37;

        std::cout << "\nWheel spins...\n";
        spinWheel(result);

        std::cout << "\nResult: ";
        printNumber(result);
        std::cout << "\n";

        if (result == betNumber) {
            int win = betAmount * 35;
            bank.deposit(win);

            std::cout << GREENR << "You Won +" << win << "\n" << RESETR;
        } else {
            std::cout << REDR << "You Lost -" << betAmount << "\n" << RESETR;
        }

        bank.print();
    }
};




class DiceGame : public IGame {
private:
    Bank& bank;

public:
    DiceGame(Bank& b) : bank(b) {}

    std::string getName() const override {
        return "Dice Game";
    }

    void play() override {
        int bet;

        std::cout << CYAN << "\n=== DICE GAME ===\n" << RESET;
        bank.print();

        std::cout << "Bet: ";
        std::cin >> bet;

        if (!bank.withdraw(bet)) {
            std::cout << RED << "Not enough Balance\n" << RESET;
            return;
        }

        int player = (std::rand() % 6) + 1;
        int cpu    = (std::rand() % 6) + 1;

        std::cout << "You:  " << player << "\n";
        std::cout << "CPU: " << cpu << "\n";

        if (player > cpu) {
            int win = bet * 2;
            bank.deposit(win);
            std::cout << GREEN << "You Won +" << win << "\n" << RESET;
        } else if (player == cpu) {
            bank.deposit(bet);
            std::cout << YELLOW << "Tie\n" << RESET;
        } else {
            std::cout << RED << "You Lost -" << bet << "\n" << RESET;
        }
    }
};


class SlotGame : public IGame {
private:
    Bank& bank;

    const int WIDTH = 5;

    std::vector<std::string> symbols = {
        "7", "BAR", "CHR", "LEM", "DIA"
    };

    std::string pad(const std::string& s) {
        std::string out = s;
        while ((int)out.size() < WIDTH) out += " ";
        return out;
    }

    std::string color(const std::string& s) {
        if (s == "7")   return "\033[31m" + pad(s) + "\033[0m";
        if (s == "BAR") return "\033[37m" + pad(s) + "\033[0m";
        if (s == "CHR") return "\033[32m" + pad(s) + "\033[0m";
        if (s == "LEM") return "\033[33m" + pad(s) + "\033[0m";
        if (s == "DIA") return "\033[36m" + pad(s) + "\033[0m";
        return pad(s);
    }

    void drawReels(int r1, int r2, int r3, bool highlight = false) {
        std::cout << "\033[H";

        std::cout << "==============================\n";
        std::cout << "         SLOT MACHINE         \n";
        std::cout << "==============================\n\n";

        bank.print();
        std::cout << "\n";

        for (int row = -1; row <= 1; row++) {
            std::string offset = "     ";
            if (row == 0) offset = "";

            int i1 = (r1 + row + symbols.size()) % symbols.size();
            int i2 = (r2 + row + symbols.size()) % symbols.size();
            int i3 = (r3 + row + symbols.size()) % symbols.size();

            if (row == 0) std::cout << "  -->";

            std::cout << offset + "  | ";

            auto cell = [&](int idx) {
                bool hl = (highlight && row == 0);

                if (hl) std::cout << "\033[42m";
                std::cout << color(symbols[idx]);
                if (hl) std::cout << "\033[0m";
            };

            cell(i1);
            std::cout << " | ";
            cell(i2);
            std::cout << " | ";
            cell(i3);

            std::cout << " |";

            if (row == 0) std::cout << "  <--";

            std::cout << "\n";
        }

        std::cout << "\n" << std::flush;
    }

    bool almostWin(const std::string& a,
                   const std::string& b,
                   const std::string& c) {
        return (a == b && b != c) ||
               (a == c && a != b) ||
               (b == c && a != b);
    }

public:
    SlotGame(Bank& b) : bank(b) {}

    std::string getName() const override {
        return "Slot Machine";
    }

    void play() override {
        std::cout << "\033[2J\033[H";
        int bet;

        std::cout << "\n=== SLOT MACHINE ===\n";
        bank.print();

        std::cout << "Bet: ";
        std::cin >> bet;

        if (!bank.withdraw(bet)) {
            std::cout << "Not enough Balance\n";
            return;
        }

        sys.restoreScreen();

        int r1 = 0, r2 = 0, r3 = 0;

        bool stop1 = false;
        bool stop2 = false;
        bool stop3 = false;

        int spins1 = 50 + std::rand() % 10;
        int spins2 = 70 + std::rand() % 10;
        int spins3 = 90 + std::rand() % 10;

        int target1 = (r1 + spins1) % symbols.size();
        int target2 = (r2 + spins2) % symbols.size();
        int target3 = (r3 + spins3) % symbols.size();

        int i = 0;

        while (!stop1 || !stop2 || !stop3) {

            if (!stop1) {
                r1 = (r1 + 1) % symbols.size();
                if (i >= spins1 && r1 == target1) {
                    stop1 = true;
                }
            }

            if (!stop2) {
                r2 = (r2 + 1) % symbols.size();
                if (i >= spins2 && r2 == target2) {
                    stop2 = true;
                }
            }

            if (!stop3) {
                r3 = (r3 + 1) % symbols.size();
                if (i >= spins3 && r3 == target3) {
                    stop3 = true;
                }
            }

            drawReels(r1, r2, r3);

            std::this_thread::sleep_for(
                std::chrono::milliseconds(20 + i * 2)
            );

            i++;
        }

        drawReels(r1, r2, r3);

        std::string a = symbols[r1];
        std::string b = symbols[r2];
        std::string c = symbols[r3];

        bool win3 = (a == b && b == c);
        bool win2 = almostWin(a, b, c);

        if (win3 || win2) {
            for (int i = 0; i < 8; i++) {
                bool on = (i % 2 == 0);

                drawReels(r1, r2, r3, on);

                std::cout << (win3 ? "\033[32mJACKPOT LINE\033[0m"
                                : "\033[33mWIN LINE\033[0m")
                        << std::flush;

                std::this_thread::sleep_for(std::chrono::milliseconds(100));
            }
        }

        std::cout << "\n";

        if (a == b && b == c) {
            int win = bet * 10;
            bank.deposit(win);
            std::cout << "\033[32mJACKPOT +" << win << "\033[0m\n";
        }
        else if (almostWin(a, b, c)) {
            int win = bet * 3;
            bank.deposit(win);
            std::cout << "\033[32mWin +" << win << "\033[0m\n";
        }
        else {
            std::cout << "\033[31mLose -" << bet << "\033[0m\n";
        }

        bank.print();
    }
};










static Bank bank(100000000);

class Casino {
private:
    std::vector<std::unique_ptr<IGame>> games;

public:
    Casino() {
        std::srand(static_cast<unsigned>(std::time(nullptr)));

        games.push_back(std::make_unique<Roulette>(bank));
        games.push_back(std::make_unique<DiceGame>(bank));
        games.push_back(std::make_unique<SlotGame>(bank));
    }

    void showMenu() {
        std::cout << "\033[2J\033[H";

        printScreen("Casino");

        bank.print();
        std::cout << "\n";

        std::cout << "0. Exit\n";

        for (size_t i = 0; i < games.size(); i++) {
            std::cout << (i + 1) << ". " << games[i]->getName() << "\n";
        }

        std::cout << "\nChoice: ";
    }

    void run() {
        while (true) {

            if (bank.getBalance() <= 0) {
                std::cout << "\033[31mNot enough Balance\n\033[0m";
                return;
            }

            showMenu();

            int choice;
            std::cin >> choice;

            if (choice == 0) {
                std::cout << "Bye\n";
                return;
            }

            if (choice < 1 || choice > (int)games.size()) {
                std::cout << "\033[31mInvalid Choice\n\033[0m";
                continue;
            }

            int currentGame = choice - 1;

            while (true) {

                games[currentGame]->play();

                if (bank.getBalance() <= 0) {
                    std::cout << "\033[31mNot enough Balance\n\033[0m";
                    return;
                }

                std::cout << "\n";
                std::cout << "\033[36m==============================\n";
                std::cout << "         NEXT STEP            \n";
                std::cout << "==============================\033[0m\n";

                std::cout << "1. Play again\n";
                std::cout << "2. Choose another game\n";
                std::cout << "3. Exit Casino\n";
                std::cout << "\nChoice: ";

                int next;
                std::cin >> next;

                if (next == 1) {
                    continue;
                }
                else if (next == 2) {
                    break;
                }
                else if (next == 3) {
                    std::cout << "Bye\n";
                    return;
                }
                else {
                    std::cout << "\033[31mInvalid Choice\033[0m\n";
                }
            }
        }
    }
};




// Startguthaben
static Casino casino;


void cmd_games(const std::vector<std::string>& args, Terminal& term) {
    (void)term;

    if (args.size() < 2) {
        help.printHelp("games", {"casino"}, false, "", true);
        return;
    }

    if (args[1] == "casino") {
        casino.run();
    }
}










class TermMenu {
    private: 
        struct MenuItem {
            std::string label;
            std::function<void()> action;
        };

        std::vector<MenuItem> menu;
        std::string title;

        void getNext() {
            std::cout << "\n";
            std::cout << currentColor << "Press Enter to Continue ...";
            std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
            std::cin.get();

            sys.restoreScreen();
        };

        void render() {
            printScreen(title);

            std::cout << "0. Exit\n";

            for (size_t i = 0; i < menu.size(); i++) {
                std::cout << i + 1 << ". " << menu[i].label << "\n";
            }
        }

    public: 
        TermMenu(std::string menuTitle)
            : title(menuTitle) {}

        void setOptions(std::vector<MenuItem> newMenu) {
            menu.clear();

            menu = newMenu;
        }

        void start() {
            while (true) {
                sys.restoreScreen();
                render(); 

                std::cout << "Choice: ";

                int choice;
                std::cin >> choice;

                if (choice == 0) {
                    sys.clearScreen();
                    break;
                }

                if (choice < 0 || choice > (int)menu.size()) {
                    std::cout << "Invalid Number\n";
                    getNext();
                    continue;
                }

                std::cout << "\n";

                menu[choice - 1].action();

                getNext();
            }
        }
};






struct MenuItem {
    std::string label;
    std::function<void()> action;
};

void cmd_bank(const std::vector<std::string>& args, Terminal& term) {
    (void)args;
    (void)term;

    TermMenu termMenu("Bank");

    termMenu.setOptions(
        {
            {
                "Show Balance",
                [&]() {
                    bank.print();
                }
            },
            {
                "Show Transactions",
                [&]() {
                    bank.printHistory();
                }
            },
            {
                "Deposit",
                [&]() {
                    int amount;
                    std::cout << "Amount: ";
                    std::cin >> amount;

                    bank.deposit(amount);
                    std::cout << "OK\n";
                }
            },
            {
                "Withdraw",
                [&]() {
                    int amount;
                    std::cout << "Amount: ";
                    std::cin >> amount;

                    if (bank.withdraw(amount)) {
                        std::cout << "OK\n";
                    } else {
                        std::cout << "Not enough Balance\n";
                    }
                }
            }
        }
    );

    termMenu.start();
}

void cmd_bankOld(const std::vector<std::string>& args, Terminal& term) {
    (void)args;
    (void)term;

    auto getNext = [&]() {
        std::cout << "\n";
        std::cout << currentColor << "Press Enter to Continue ...";
        std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
        std::cin.get();

        sys.restoreScreen();
    };

    sys.clearScreen();

    while (true) {
        std::vector<MenuItem> menu = {
            {
                "Show Balance",
                [&]() {
                    bank.print();
                    getNext();
                }
            },
            {
                "Show Transactions",
                [&]() {
                    bank.printHistory();
                    getNext();
                }
            },
            {
                "Deposit",
                [&]() {
                    int amount;
                    std::cout << "Amount: ";
                    std::cin >> amount;

                    bank.deposit(amount);
                    std::cout << "OK\n";

                    getNext();
                }
            },
            {
                "Withdraw",
                [&]() {
                    int amount;
                    std::cout << "Amount: ";
                    std::cin >> amount;

                    if (bank.withdraw(amount)) {
                        std::cout << "OK\n";
                    } else {
                        std::cout << "Not enough Balance\n";
                    }

                    getNext();
                }
            }
        };

        sys.restoreScreen();

        printScreen("Bank");

        std::cout << "0. Exit\n";

        for (size_t i = 0; i < menu.size(); i++) {
            std::cout << i + 1 << ". " << menu[i].label << "\n";
        }

        std::cout << "Choice: ";

        int choice;
        std::cin >> choice;

        if (choice == 0) {
            sys.clearScreen();
            break;
        }

        if (choice < 0 || choice > (int)menu.size()) {
            std::cout << "Invalid Number\n";
            getNext();
            continue;
        }

        std::cout << "\n";

        menu[choice - 1].action();
    }
}
















class AsciiGraph {
private:
    struct point {
        std::string label;
        double y;
    };

    struct Point {
        double x;
        double y;
    };

    std::vector<Point> points;
    std::vector<std::string> xLabels;

    int width;
    int plotWidth;
    int height;

    int cellWidth = 3;

    std::vector<std::string> grid;

    double minX, maxX;
    double minY, maxY;

    void computeBounds() {
        minX = maxX = points[0].x;
        minY = maxY = points[0].y;

        for (const auto& p : points) {
            minX = std::min(minX, p.x);
            maxX = std::max(maxX, p.x);
            minY = std::min(minY, p.y);
            maxY = std::max(maxY, p.y);
        }

        if (minY == maxY) {
            minY -= 1;
            maxY += 1;
        }

        if (minX == maxX) {
            minX -= 1;
            maxX += 1;
        }
    }

    std::vector<double> computeTicks() const {
        std::vector<double> ticks;

        double range = maxY - minY;
        if (range <= 0) return {minY};

        double rawStep = range / 5.0;

        double mag = std::pow(10, std::floor(std::log10(rawStep)));
        double norm = rawStep / mag;

        double step;

        if (norm < 1.5) step = 1 * mag;
        else if (norm < 3) step = 2 * mag;
        else if (norm < 7) step = 3 * mag;
        else step = 10 * mag;

        double start = std::floor(minY / step) * step;

        for (double v = start; v <= maxY; v += step) {
            ticks.push_back(v);
        }

        return ticks;
    }

    static double computeStep(double minY, double maxY) {
        double range = maxY - minY;
        double rawStep = range / 6.0;

        double mag = std::pow(10, std::floor(std::log10(rawStep)));
        double norm = rawStep / mag;

        if (norm < 1.5) return 1 * mag;
        if (norm < 3)   return 2 * mag;
        if (norm < 7)   return 5 * mag;
        return 10 * mag;
    }

    int mapX(double x) const {
        double t = (x - minX) / (maxX - minX);
        return (int)(t * (width * cellWidth - 1));
    }

    int mapY(double y) const {
        return (int)((1.0 - (y - minY) / (maxY - minY)) * (height - 1));
    }

    int mapTickToRow(double tick) const {
        return (int)((maxY - tick) / (maxY - minY) * (height - 1));
    }

    void initGrid() {
        grid.assign(height, std::string(width * cellWidth, ' '));
    }

    void setPoint(int x, int y, char c) {
        if (x < 0 || x >= width * cellWidth || y < 0 || y >= height) return;

        grid[y][x] = c;
    }

    void drawLine(int x0, int y0, int x1, int y1) {
        int dx = std::abs(x1 - x0);
        int dy = std::abs(y1 - y0);

        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;

        int err = dx - dy;

        while (true) {
            if (x0 == x1 && y0 == y1) break;
            
            setPoint(x0, y0, '*');

            int err2 = err;

            if (err2 > -dy) {
                err -= dy;
                x0 += sx;
            } else if (err2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }

public:
    AsciiGraph(int h = 20)
        : height(h) {}

    void addPoints(std::vector<point> allPoints) {
        for (size_t i = 0; i < allPoints.size(); i++) {
            points.push_back({(double)i, allPoints[i].y});
            xLabels.push_back(allPoints[i].label);
        }
    }

    void setXLabels(const std::vector<std::string>& labels) {
        xLabels = labels;
    }

    void prepare() {
        computeBounds();

        width = (int)points.size();
        plotWidth = width * cellWidth;
        if (width < 2) width = 2;

        initGrid();
    }

    void drawAxes() {
        int zeroY = mapY(0);
        int zeroX = mapX(0);

        for (int x = 0; x < width * cellWidth; x++) {
            if (zeroY >= 0 && zeroY < height) {
                grid[zeroY][x] = '-';
            }
        }

        if (zeroX >= 0 && zeroX < width &&
            zeroY >= 0 && zeroY < height) {
            grid[zeroY][zeroX * cellWidth + 1] = '+';
        }
    }

    void plot() {
        grid.assign(height, std::string(plotWidth, ' '));

        for (size_t i = 0; i + 1 < points.size(); i++) {
            int x0 = mapX(points[i].x);
            int y0 = mapY(points[i].y);

            int x1 = mapX(points[i + 1].x);
            int y1 = mapY(points[i + 1].y);

            drawLine(x0, y0, x1, y1);
        }
    }

    void print() const {
        std::string spacing = "       ";
        double step = computeStep(minY, maxY);

        int tickSpacing = height / 6;
        if (tickSpacing < 1) tickSpacing = 1;

        for (int y = 0; y < height; y++) {

            bool isTick = (y % tickSpacing == 0);

            if (isTick) {
                double value = maxY - (double)y / (height - 1) * (maxY - minY);

                double rounded = std::round(value / step) * step;

                std::cout << std::setw(6)
                        << std::fixed << std::setprecision(2)
                        << rounded << " |";
            } else {
                std::cout << spacing << "|";
            }

            std::cout << grid[y] << "\n";
        }

        std::cout << spacing << "+";

        for (int i = 0; i < width * cellWidth; i++) {
            std::cout << "-";
        }

        std::cout << "\n" << spacing << " ";

        for (int i = 0; i < width; i++) {
            std::string label;

            if (i < (int)xLabels.size()) {
                label = xLabels[i];
            } else {
                label = std::to_string(i);
            }

            int padding = cellWidth - (int)label.size();
            int left = padding / 2;

            for (int j = 0; j < left; j++) std::cout << " ";
            std::cout << label;
            for (int j = 0; j < padding - left; j++) std::cout << " ";
        }

        std::cout << "\n";
    }
};

class ProgressBar {
    private: 
        int total = 100;
        int barWidth;

    public: 
        ProgressBar(int width)
            :barWidth(width) {}

        void print(int progress) {
            float percent = (float)progress / total;
            int pos = barWidth * percent;

            std::cout << "[";
            for (int i = 0; i < barWidth; ++i) {
                if (i < pos)
                    std::cout << "=";
                else if (i == pos)
                    std::cout << ">";
                else
                    std::cout << " ";
            }
            std::cout << "] " << int(percent * 100.0) << " %\r";
            std::cout.flush();
        }

        void render() {
            for (int i = 0; i <= total; ++i) {
                print(i);
                std::this_thread::sleep_for(std::chrono::milliseconds(50));
            }

            std::cout << std::endl;
        }
};



void cmd_graph(const std::vector<std::string>& args, Terminal& term) {
    (void) term;

    if ((int) args.size() < 2) {
        help.printHelp("graph", {"line", "bar"}, false, "", true);
        return;
    }
    
    if (args[1] == "line") {
        AsciiGraph g(16);

        g.addPoints( 
            {
                {"A", 1},
                {"B", 3},
                {"C", 2},
                {"D", 5},
                {"E", 4},
                {"F", 6},
                {"G", 3},
                {"H", 2},
                {"I", 4},
                {"J", 5},
                {"K", 6}
            }
        );

        g.prepare();
        g.drawAxes();
        g.plot();
        g.print();
    } else if (args[1] == "bar") {
        ProgressBar bar(50);
        bar.render();
    }
}



















void cmd_cat(const std::vector<std::string>& args, Terminal& term) {
    (void) term;
    if (args.size() < 2) {
        std::cerr << "cat: missing file operand\n";
        return;
    }

    bool showNumbers = false;
    int startIndex = 1;

    if (std::string(args[1]) == "-n") {
        showNumbers = true;
        startIndex = 2;
    }

    for (size_t i = startIndex; i < args.size(); i++) {
        std::ifstream file(args[i]);

        if (!file.is_open()) {
            std::cerr << "cat: cannot open " << args[i] << "\n";
            continue;
        }

        std::string line;
        int lineNumber = 1;

        while (std::getline(file, line)) {
            if (showNumbers) {
                std::cout << lineNumber++ << "  ";
            }
            std::cout << line << std::endl;
        }

        file.close();
    }

}















void cmd_db(const std::vector<std::string>& args, Terminal& term) {
    (void) term;

    if (args.size() < 2) {
        help.printHelp("db", {"create", "insert", "query", "list", "drop", "clear"}, false, "", true);
        return;
    }

    std::string sub = args[1];

    if (sub == "create") {
        if (args.size() < 4) {
            help.printHelp("db create", {"name", "col1", "col2", "..."}, false, "", true);
            return;
        }

        std::string name = args[2];

        std::vector<std::string> columns;
        for (size_t i = 3; i < args.size(); i++) {
            columns.push_back(args[i]);
        }

        db.create(name, columns);
    } else if (sub == "insert") {
        if (args.size() < 4) {
            help.printHelp("db insert", {"name", "value1", "value2", "..."}, false, "", true);
            return;
        }

        std::string name = args[2];

        std::vector<std::string> values;
        for (size_t i = 3; i < args.size(); i++) {
            values.push_back(args[i]);
        }

        db.insert(name, values);
    } else if (sub == "query") {
        std::string name = args[2];

        std::vector<DataBase::Condition> conditions;
        std::vector<std::string> logic;
        std::string orderCol;

        for (size_t i = 3; i < args.size(); i++) {

            if (args[i] == "where") {
                i++;
                while (i < args.size()) {

                    if (args[i] == "orderby") break;

                    if (args[i] == "and" || args[i] == "or") {
                        logic.push_back(args[i]);
                    } else {
                        conditions.push_back(db.parseCondition(args[i]));
                    }

                    i++;
                }
                i--;
            }

            else if (args[i] == "orderby" && i + 1 < args.size()) {
                orderCol = args[i + 1];
                i++;
            }
        }

        db.queryAdvanced(name, conditions, logic, orderCol);
    } else if (sub == "list") {
        db.list();
    } else if (sub == "drop") {
        if (args.size() < 3) {
            help.printHelp("db drop", {"name"}, false, "", true);
            return;
        }

        db.drop(args[2]);
    } else if (sub == "clear") {
        if (args.size() < 3) {
            help.printHelp("db clear", {"name"}, false, "", true);
            return;
        }

        db.clear(args[2]);
    }

    else {
        help.printHelp("db", {"create", "insert", "query", "list", "drop", "clear"}, false, "", true);
    }
}  

















class ManManager: public Config {
private:
    const std::string colTitle   = getAnsiColor('B');
    const std::string colSection = getAnsiColor('D');
    const std::string colLabel   = getAnsiColor('1');
    const std::string colValue   = getAnsiColor('F');
    const std::string colError   = getAnsiColor('C');
    const std::string colLine    = getAnsiColor('8');
    const std::string reset      = "\033[0m";

    std::vector<ManPage> manPages;

    int maxPerLine = 5;

    bool findPage(const std::string& name, ManPage& out) const {
        for (const auto& p : manPages) {
            if (p.name == name) {
                out = p;
                return true;
            }
        }
        return false;
    }

    int calculateWidth(const ManPage& p) const {
        int width = 40;

        auto check = [&](const std::string& label, const std::string& value) {
            int len = 2 + label.size() + 1 + value.size();
            if (len > width) width = len;
        };

        check(p.name, p.description);
        check("command", p.usage);

        for (const auto& opt : p.options) {
            check(opt.flag, opt.description);
        }

        for (const auto& ex : p.examples) {
            check("$", ex.example);
        }

        return width + 4;
    }

    void line(int width, char c = '=') const {
        std::cout << colLine << std::string(width, c) << reset << "\n";
    }

    void center(int width, const std::string& text) const {
        int pad = (width - text.size()) / 2;
        if (pad < 0) pad = 0;

        std::cout << std::string(pad, ' ')
                  << colTitle << text << reset << "\n";
    }

    void section(const std::string& name) const {
        std::cout << colSection << "\n" << name << ":\n" << reset;
    }

    void row(int labelWidth, const std::string& label, const std::string& value) const {
        std::cout << "    "
                  << colLabel << std::setw(labelWidth) << std::left << label << reset
                  << " "
                  << colValue << value << reset << "\n";
    }

public:
    void initManPages() {
        manPages.clear();

        for (const auto& cmd : getManPage()) {
            manPages.push_back({
                cmd.name,
                cmd.description,
                cmd.usage,
                cmd.options,
                cmd.examples
            });
        }
    }

    void showPage(const std::string& name) const {
        ManPage p;

        if (!findPage(name, p)) {
            std::cout << colError
                    << "No manual entry for '" << name << "'\n"
                    << reset;
            return;
        }

        int width = calculateWidth(p);
        int labelWidth = 15;

        line(width);
        center(width, "Man Page: " + p.name);
        line(width);

        section("Name");
        row(labelWidth, p.name, p.description);

        section("Usage");
        row(labelWidth, "command", p.usage);

        if (!p.options.empty()) {
            section("Options");

            for (const auto& opt : p.options) {
                row(labelWidth, opt.flag, opt.description);
            }
        }

        if (!p.examples.empty()) {
            section("Examples");

            for (const auto& ex : p.examples) {
                row(labelWidth, "$", ex.example);
            }
        }

        line(width);
    }

    void listCommands() const {
        int minWidth = 30;
        int width = minWidth;

        for (const auto& page : manPages) {
            int len = page.name.size() + 4;
            if (len > width) width = len;
        }

        line(width);
        center(width, "Available Commands");
        line(width);

        int currentLineLength = 2;

        std::cout << "  ";

        for (size_t i = 0; i < manPages.size(); i++) {
            std::string output = manPages[i].name;

            bool isLast = (i == (manPages.size() - 1));
            if (!isLast) output += reset + ", ";

            int needed = (output.size() - reset.size());

            if ((currentLineLength + needed) > width) {
                std::cout << "\n  ";
                currentLineLength = 2;
            }

            std::cout << colLabel << output << reset;
            currentLineLength += needed;
        }

        std::cout << "\n";
        line(width);
    }
};

void cmd_man(const std::vector<std::string>& args, Terminal& term) {
    (void) term;
    ManManager man;

    man.initManPages();

    if (args.size() < 2) man.listCommands();
    else if (args.size() == 2) man.showPage(args[1]);
    else help.printHelp("man", {"cmd"}, false, "", true);
}








struct ShapeArgs {
    std::string title;
    int minSize = 0;
};
struct ShapeFunc {
    std::function<void(int)> filled = [](int) {};
    std::function<void(int)> empty = [](int) {};
    std::function<void()> standard = []() {};
};
struct Shape {
    ShapeArgs args;
    ShapeFunc func;
};

class Draw {
    public: 
        const std::string COLOR_BORDER = getAnsiColor('B');
        const std::string COLOR_LINE   = getAnsiColor('F');
        const std::string COLOR_CMD    = getAnsiColor('7');
        const std::string COLOR_ERROR  = getAnsiColor('4');
        const std::string COLOR_OK     = getAnsiColor('A');
        const std::string COLOR_INFO   = getAnsiColor('9');
        const std::string COLOR_DESC   = getAnsiColor('8');
        const std::string COLOR_RESET  = "\033[0m";
    private: 
        size_t columns;
        size_t rows;
        CONSOLE_SCREEN_BUFFER_INFO csbi;

        void calcScreenSize() {
            GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
            columns = csbi.srWindow.Right - csbi.srWindow.Left + 1;
            rows = csbi.srWindow.Bottom - csbi.srWindow.Top + 1;

            (void)columns;
        }

        void printHorizontal(std::string left, std::string mid, std::string right) {
            std::cout << COLOR_BORDER << left;

            for (size_t i = 0; i < columns - 2; i++)
                std::cout << mid;

            std::cout << right << "\n" << COLOR_RESET;
        }

        // Source - https://stackoverflow.com/a/17976083
        // Posted by szx, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-06-09, License - CC BY-SA 3.0

        bool is_number(const std::string &s) {
            return !s.empty() && std::all_of(s.begin(), s.end(), ::isdigit);
        }


    public:
        int getIntInput(std::string prompt) {
            int value;
            std::cout << COLOR_CMD << prompt << ": " << COLOR_RESET;
            std::cin >> value;
            return value;
        }

        void handleShape(const std::vector<std::string>& args, 
                        Shape shape) {
            std::string underline = "\033[4m";
            std::string shapeTitle = args[1];
            if (!shapeTitle.empty())
                shapeTitle[0] = static_cast<char>(
                    std::toupper(static_cast<unsigned char>(shapeTitle[0]))
                );

            if (args.size() < 3 && shape.args.minSize > 0) {
                help.printHelp("draw " + args[1], {"filled size", "size"}, false, "", true);
                return;
            }
            
            if (shape.args.minSize > 0) {
                if (args[2] == "filled") {
                    if (args.size() < 4) {
                        help.printHelp("draw " + args[1] + " filled", {"size"}, false, "", true);
                        return;
                    } 
                    
                    if (!is_number(args[3])) {
                        std::cout << COLOR_ERROR << "Size of " 
                                    << COLOR_DESC << shapeTitle 
                                    << COLOR_ERROR << " can only be a Number that is " 
                                    << underline << "larger" << COLOR_RESET 
                                    << COLOR_ERROR << " than " 
                                    << COLOR_INFO << shape.args.minSize << "\n" << COLOR_RESET;
                        return;
                    } else if (std::stoi(args[3]) < shape.args.minSize) {
                            std::cout << COLOR_ERROR << "Size of " 
                                    << COLOR_DESC << shapeTitle 
                                    << COLOR_ERROR << " must be " 
                                    << underline << "at least" << COLOR_RESET 
                                    << COLOR_ERROR << " " 
                                    << COLOR_INFO << shape.args.minSize << "\n" << COLOR_RESET;
                        return;
                    } else if (std::stoi(args[3]) >= shape.args.minSize) {
                        shape.func.filled(std::stoi(args[3]));
                    } else {
                        help.printHelp("draw " + args[1] + " filled", {"size"}, false, "", true);
                        return;
                    }
                } else if (!is_number(args[2])) {
                    std::cout << COLOR_ERROR << "Size of " 
                                << COLOR_DESC << shapeTitle 
                                << COLOR_ERROR << " can only be a Number that is " 
                                << underline << "larger" << COLOR_RESET 
                                << COLOR_ERROR << " than " 
                                << COLOR_INFO << shape.args.minSize << "\n" << COLOR_RESET;
                    return;
                } else if (std::stoi(args[2]) < shape.args.minSize) {
                    std::cout << COLOR_ERROR << "Size of " 
                                << COLOR_DESC << shapeTitle 
                                << COLOR_ERROR << " must be " 
                                << underline << "at least" << COLOR_RESET 
                                << COLOR_ERROR << " " 
                                << COLOR_INFO << shape.args.minSize << "\n" << COLOR_RESET;
                    return;
                } else if (std::stoi(args[2]) >= shape.args.minSize) {
                    shape.func.empty(std::stoi(args[2]));
                } else {
                    help.printHelp("draw " + args[1], {"filled size", "size"}, false, "", true);
                    return;
                }
            } else {
                shape.func.standard();
            }
        }

        void frame() {
            calcScreenSize();

            printHorizontal("┌", "─", "┐");

            for (size_t i = 0; i < rows - 3; i++) 
                printHorizontal("│", " ", "│");

            printHorizontal("└", "─", "┘");
        }

        void squareFilled(int size) {
            if (size < 6)
                return;

            const int width  = size * 2;
            const int height = size;

            const int cx = width / 2;
            const int cy = height / 2;

            const double maxDist = std::sqrt(cx * cx + cy * cy);

            for (int y = 0; y < height; ++y) {
                std::cout << COLOR_BORDER;

                for (int x = 0; x < width; ++x) {
                    bool isTop    = (y == 0);
                    bool isBottom = (y == height - 1);
                    bool isLeft   = (x == 0);
                    bool isRight  = (x == width - 1);

                    if (x == 0 && y == 0)
                        std::cout << "┏";
                    else if (x == width - 1 && y == 0)
                        std::cout << "┓";
                    else if (x == 0 && y == height - 1)
                        std::cout << "┗";
                    else if (x == width - 1 && y == height - 1)
                        std::cout << "┛";
                    else if (isTop || isBottom)
                        std::cout << "━";
                    else if (isLeft || isRight)
                        std::cout << "┃";
                    else {
                        int dx = x - cx;
                        int dy = y - cy;

                        double r = std::sqrt(dx * dx + dy * dy);

                        double nx = dx / (double)cx;
                        double ny = dy / (double)cy;

                        double radial = 1.0 - (r / maxDist);

                        double diagonal = 1.0 - std::abs(nx + ny) * 0.5;

                        double bands =
                            std::sin(r * 0.6) * 0.35 +
                            std::cos((nx - ny) * 3.0) * 0.25;

                        double value = radial * 0.6 + diagonal * 0.25 + bands * 0.15;

                        std::string c;

                        if (value > 0.75)
                            c = "█";
                        else if (value > 0.6)
                            c = "▓";
                        else if (value > 0.45)
                            c = "▒";
                        else if (value > 0.3)
                            c = "░";
                        else
                            c = " ";

                        std::cout << c;
                    }
                }

                std::cout << COLOR_RESET << '\n';
            }
        }

        void squareEmpty(int size) {
            if (size < 6)
                return;

            const int width  = size * 2;
            const int height = size;

            for (int y = 0; y < height; ++y) {
                std::cout << COLOR_BORDER;

                for (int x = 0; x < width; ++x) {
                    bool isTop    = (y == 0);
                    bool isBottom = (y == height - 1);
                    bool isLeft   = (x == 0);
                    bool isRight  = (x == width - 1);

                    if (x == 0 && y == 0)
                        std::cout << "┏";
                    else if (x == width - 1 && y == 0)
                        std::cout << "┓";
                    else if (x == 0 && y == height - 1)
                        std::cout << "┗";
                    else if (x == width - 1 && y == height - 1)
                        std::cout << "┛";
                    else if (isTop || isBottom)
                        std::cout << "━";
                    else if (isLeft || isRight)
                        std::cout << "┃";
                    else
                        std::cout << " ";
                }

                std::cout << COLOR_RESET << '\n';
            }
        }

        void triangleFilled(int size) {
            if (size < 6)
                return;

            const int width  = size * 2 - 1;
            const int height = size;

            const double cx = width / 2.0;

            for (int y = 0; y < height; ++y) {
                std::cout << COLOR_BORDER;

                for (int x = 0; x < width; ++x) {
                    const int leftEdge  = (int)(cx - y);
                    const int rightEdge = (int)(cx + y);

                    const bool top    = (y == 0 && x == (int)cx);
                    const bool bottom = (y == height - 1);
                    const bool left   = (x == leftEdge);
                    const bool right  = (x == rightEdge);

                    if (top) {
                        std::cout << "╻";
                    } else if (bottom) {
                        if (x == leftEdge)
                            std::cout << "┗";
                        else if (x == rightEdge)
                            std::cout << "┛";
                        else
                            std::cout << "━";
                    } else if (left) {
                        std::cout << "╱";
                    } else if (right) {
                        std::cout << "╲";
                    } else if (x > leftEdge && x < rightEdge) {
                        int dx = x - cx;
                        int dy = y;

                        double r = std::sqrt(dx * dx + dy * dy);

                        double maxR = std::sqrt((cx * cx) + (height * height));

                        double radial = 1.0 - (r / maxR);

                        double vertical = 1.0 - (y / (double)height);

                        double wave =
                            std::sin(x * 0.35) * 0.25 +
                            std::cos((x + y) * 0.25) * 0.25;

                        double diagonal =
                            1.0 - std::abs((x - cx) / (y + 1.0)) * 0.6;

                        double value =
                            radial * 0.45 +
                            vertical * 0.25 +
                            wave * 0.20 +
                            diagonal * 0.10;

                        std::string c;

                        if (value > 0.75)
                            c = "█";
                        else if (value > 0.6)
                            c = "▓";
                        else if (value > 0.45)
                            c = "▒";
                        else if (value > 0.3)
                            c = "░";
                        else
                            c = " ";

                        std::cout << c;
                    } else {
                        std::cout << " ";
                    }
                }

                std::cout << COLOR_RESET << '\n';
            }
        }

        void triangleEmpty(int size) {
            if (size < 6)
                return;

            const int width  = size * 2 - 1;
            const int height = size;

            const double cx = width / 2.0;

            for (int y = 0; y < height; ++y) {
                std::cout << COLOR_BORDER;

                for (int x = 0; x < width; ++x) {
                    const int leftEdge  = (int)(cx - y);
                    const int rightEdge = (int)(cx + y);

                    const bool top    = (y == 0 && x == (int)cx);
                    const bool bottom = (y == height - 1);
                    const bool left   = (x == leftEdge);
                    const bool right  = (x == rightEdge);

                    if (top) {
                        std::cout << "╻";
                    } else if (bottom) {
                        if (x == leftEdge)
                            std::cout << "┗";
                        else if (x == rightEdge)
                            std::cout << "┛";
                        else
                            std::cout << "━";
                    } else if (left) {
                        std::cout << "╱";
                    } else if (right) {
                        std::cout << "╲";
                    } else {
                        std::cout << " ";
                    }
                }

                std::cout << COLOR_RESET << '\n';
            }
        }
};

class drawManager {
    private: 
        Draw draw;
        std::vector<Shape> shapes;
        
    public: 
        void setShapes(std::vector<Shape> newShapes) {
            shapes = newShapes;
        }

        void handleUserInput(const std::vector<std::string>& args) {
            for (const Shape& shape : shapes) {
                if (args.size() < 2) {
                    help.printHelp("draw", {"frame", "square", "triangle"}, false, "", true);
                    return;
                }
                if (args[1] == shape.args.title) {
                    draw.handleShape(args, shape);
                    return;
                }
            }

            help.printHelp("draw", {"frame", "square", "triangle"}, false, "", true);
        }
};

drawManager drawMan;







void cmd_draw(const std::vector<std::string>& args, Terminal& term) {
    (void) term;
    if (args.size() < 2) {
        help.printHelp("draw", {"frame", "square", "triangle"}, false, "", true);
        return;
    }

    Draw draw;

    drawMan.setShapes({
        {
            {"frame", 0},
            {   
                [&](int MinSize) { (void)MinSize; },
                [&](int MinSize) { (void)MinSize; }, 
                [&]() { draw.frame(); }
            }
        }, {
            {"square", 6},
            {   
                [&](int MinSize) { draw.squareFilled(MinSize); },
                [&](int MinSize) { draw.squareEmpty(MinSize); } 
            }
        }, {
            {"triangle", 3},
            {   
                [&](int MinSize) { draw.triangleFilled(MinSize); },
                [&](int MinSize) { draw.triangleEmpty(MinSize); } 
            }
        }
    });

    drawMan.handleUserInput(args);
}







namespace GeistScript {

    enum TokenType {
        End, Empty, NewLine, Number, StringLiteral, Identifier,
        Let, Const, Print, If, Else, While, For, Function, Return,
        Plus, Minus, Multiply, Divide, Modulo,
        Assign, Equal, NotEqual, And, Or, Not, String,
        Less, Greater, LessEqual, GreaterEqual,
        LParen, RParen, LBrace, RBrace, LBracket, RBracket,
        NewClass, New, Public, Private, Protected, Dot,
        Semicolon, Comma
    };
    
    struct Token {
        TokenType type;
        std::string text;
    };
    
    class Lexer {
        std::string src;
        size_t pos = 0;
    public:
        Lexer(const std::string& s) : src(s) {}
    
        Token next() {
            while (pos < src.size()) {

                while (pos < src.size() && std::isspace((unsigned char)src[pos]))
                    pos++;

                if (pos >= src.size())
                    return {TokenType::End, ""};

                char c = src[pos];

                if (c == '/' && pos + 1 < src.size() && src[pos + 1] == '/') {
                    pos += 2;

                    while (pos < src.size() && src[pos] != '\n')
                        pos++;

                    continue;
                }

                if (c == '/' && pos + 1 < src.size() && src[pos + 1] == '*') {
                    pos += 2;

                    while (pos + 1 < src.size()) {
                        if (src[pos] == '*' && src[pos + 1] == '/') {
                            pos += 2;
                            break;
                        }
                        pos++;
                    }

                    continue;
                }

                if (std::isdigit(c)) {
                    std::string n;
                    while (pos < src.size() && std::isdigit((unsigned char)src[pos]))
                        n += src[pos++];

                    return {TokenType::Number, n};
                }

                if (std::isalpha(c) || c == '_') {
                    std::string id;
                    while (pos < src.size() &&
                        (std::isalnum((unsigned char)src[pos]) || src[pos] == '_'))
                        id += src[pos++];

                    if (id == "let") return {TokenType::Let, id};
                    if (id == "const") return {TokenType::Const, id};
                    if (id == "print") return {TokenType::Print, id};
                    if (id == "if") return {TokenType::If, id};
                    if (id == "else") return {TokenType::Else, id};
                    if (id == "while") return {TokenType::While, id};
                    if (id == "for") return {TokenType::For, id};
                    if (id == "function") return {TokenType::Function, id};
                    if (id == "return") return {TokenType::Return, id};
                    if (id == "nl") return {TokenType::NewLine, id};
                    if (id == "class") return {TokenType::NewClass, id};
                    if (id == "public") return {TokenType::Public, id};
                    if (id == "private") return {TokenType::Private, id};
                    if (id == "protected") return {TokenType::Protected, id};
                    if (id == "new") return {TokenType::New, id};
                    return {TokenType::Identifier, id};
                }

                if (c == '"') {
                    pos++;
                    std::string s;

                    while (pos < src.size() && src[pos] != '"')
                        s += src[pos++];

                    pos++;
                    return {TokenType::StringLiteral, s};
                }

                pos++;

                switch (c) {
                    case '+':   return {TokenType::Plus, "+"};
                    case '-':   return {TokenType::Minus, "-"};
                    case '*':   return {TokenType::Multiply, "*"};
                    case '/':   return {TokenType::Divide, "/"};
                    case '%':   return {TokenType::Modulo, "%"};
                    case '(':   return {TokenType::LParen, "("};
                    case ')':   return {TokenType::RParen, ")"};
                    case '{':   return {TokenType::LBrace, "{"};
                    case '}':   return {TokenType::RBrace, "}"};
                    case '[':   return {TokenType::LBracket, "["};
                    case ']':   return {TokenType::RBracket, "]"};
                    case ';':   return {TokenType::Semicolon, ";"};
                    case ',':   return {TokenType::Comma, ","};
                    case '.':   return {TokenType::Dot, "."};
                    case '!':
                        if (src[pos] == '=') {
                            pos++;
                            return {TokenType::NotEqual, "!="};
                        }
                        return {TokenType::Not, "!"};

                    case '=':
                        if (src[pos] == '=') {
                            pos++;
                            return {TokenType::Equal, "=="};
                        }
                        return {TokenType::Assign, "="};

                    case '<':
                        if (src[pos] == '=') {
                            pos++;
                            return {TokenType::LessEqual, "<="};
                        }
                        return {TokenType::Less, "<"};

                    case '>':
                        if (src[pos] == '=') {
                            pos++;
                            return {TokenType::GreaterEqual, ">="};
                        }
                        return {TokenType::Greater, ">"};
                    case '&':
                        if (pos < src.size() && src[pos] == '&') {
                            pos++;
                            return {TokenType::And, "&&"};
                        }
                        throw std::runtime_error("Unexpected character '&'");

                    case '|':
                        if (pos < src.size() && src[pos] == '|') {
                            pos++;
                            return {TokenType::Or, "||"};
                        }
                        throw std::runtime_error("Unexpected character '|'");
                }

                throw std::runtime_error("Unexpected character: " + c);
            }

            return {TokenType::End, ""};
        }
    };

    struct Value {
        bool isConst = false;
        bool isString = false;
        long long number = 0;
        std::string str;
        int layer;
        std::string parent;
        Token className = {};

        static Value handleVal(
            std::string par, 
            int lay, 
            bool isConstant,
            std::string vS = "",
            long long vL = 0,
            Token className = {}
        ) {
            Value x;
            x.layer = lay;
            x.parent = par;
            x.className = className;
            x.isConst = isConstant;

            if (vS != "") {
                x.isString = true;
                x.str = std::move(vS);
            } else if (vL != 0) {
                x.number = vL;
            }

            return x;
        }
    };

    struct Func {
        int layer;
        std::string parent;
        std::string name;
        std::vector<Token> params;
        std::vector<Token> body;
        std::unordered_map<std::string, Value> localVars = {};
        bool hasReturned = false;
        Token returnValue = {TokenType::End, ""};
    };

    enum class AccessModifier {
        Public,
        Private,
        Protected
    };

    struct ClassVariable {
        Value value;
        AccessModifier access = AccessModifier::Private;
    };

    struct ClassFunction {
        Func func;
        AccessModifier access = AccessModifier::Private;
    };

    struct Class {
        std::string name;
        std::string parent;
        int layer;

        std::unordered_map<std::string, ClassVariable> variables = {};
        std::unordered_map<std::string, ClassFunction> functions = {};
    };
    
    class Interpreter {
    private:
        std::unordered_map<std::string, Value> vars;
        std::unordered_map<std::string, Func> funcs;
        std::unordered_map<std::string, Class> classes;

        struct ConditionResult {
            bool value;
            size_t newPos;
        };

        Token combine(std::vector<Token> tokens) {
            std::string combinedText;
            for (const auto& t : tokens) {
                combinedText += t.text + " ";
            }
            return {TokenType::Empty, combinedText};
        }

        bool isTokenType(Token token) {
            return (token.type >= TokenType::End && 
                    token.type <= TokenType::Comma && 
                    token.type != TokenType::Identifier);
        }

        bool isVarName(Token token) {
            for (auto var : vars) {
                if (var.first == token.text) {
                    return true;
                }
            }
            return false;
        }

        bool isLocalVarName(std::string parentFunc, Token token) {
            auto& func = funcs[parentFunc];
            for (auto var : func.localVars) {
                if (var.first == token.text) {
                    return true;
                }
            }
            return false;
        }

        Value tokenToValue(Token t) {
            std::string varStr = "";
            long long varNum = 0;
            Value val;

            if (t.type == TokenType::StringLiteral) {
                varStr = t.text;
            } else if (t.type == TokenType::Number) {
                varNum = std::stoll(t.text);
            } else if (t.type == TokenType::Identifier) {
                auto& var = vars[t.text];
                if (var.isString) varStr = var.str;
                else varNum = var.number;
            }

            val = Value::handleVal("root", 0, false, varStr, varNum);
            return val;
        }

        bool isFuncName(Token token) {
            for (auto func : funcs) {
                if (func.second.name == token.text) {
                    return true;
                }
            }
            return false;
        }

        bool isClassName(Token token) {
            return classes.find(token.text) != classes.end();
        }

        bool isClassVariable(Token cls, Token var) {
            if (!isClassName(cls))
                return false;

            auto &c = classes[cls.text];
            return c.variables.find(var.text) != c.variables.end();
        }

        bool isClassFunction(Token cls, Token func) {
            if (!isClassName(cls))
                return false;

            auto &c = classes[cls.text];
            return c.functions.find(func.text) != c.functions.end();
        }

        bool isClassAccess(
            const std::vector<Token>& tokens,
            size_t pos)
        {
            if (pos + 2 >= tokens.size())
                return false;

            return
                tokens[pos].type == TokenType::Identifier &&
                tokens[pos + 1].type == TokenType::Dot &&
                tokens[pos + 2].type == TokenType::Identifier;
        }

        int executeFunction(
            Token t, 
            const int Layer, 
            std::string parent, 
            int numLine, 
            std::vector<Token> tokens,
            int pos,
            std::string caller = "root",
            bool isRetCall = false,
            bool isConstructor = false
        ) {
            std::function<void(const std::vector<Token>&, std::string, int, int)> run;
            run = [&](const std::vector<Token>& toks, std::string p, int l, int n) {
                executeTokens(toks, p, l, n);
            };

            int newLayer = Layer;
            newLayer++;
            bool isClassFunc = false;
            auto& func = funcs[t.text];
            func.body.push_back({TokenType::End, ""});

            /*
                    struct Func {
                        int layer;
                        std::string parent;
                        std::string name;
                        std::vector<Token> params;
                        std::vector<Token> body;
                        std::unordered_map<std::string, Value> localVars = {};
                        bool hasReturned = false;
                        Token returnValue = {TokenType::End, ""};
                    };

                    enum class AccessModifier {
                        Public,
                        Private,
                        Protected
                    };

                    struct ClassVariable {
                        Value value;
                        AccessModifier access = AccessModifier::Private;
                    };

                    struct ClassFunction {
                        Func func;
                        AccessModifier access = AccessModifier::Private;
                    };

                    struct Class {
                        std::string name;
                        std::string parent;
                        int layer;

                        std::unordered_map<std::string, ClassVariable> variables;
                        std::unordered_map<std::string, ClassFunction> functions;
                    };
                    */

            if (isClassName({TokenType::Empty, parent}) && !isConstructor) {
                Token funcName = tokens[pos++];
                isClassFunc = true;
                func = classes[parent].functions[funcName.text].func;
            } else if (isClassName({TokenType::Empty, parent}) && isConstructor) {
                isClassFunc = true;
                func = classes[parent].functions["constructor"].func;
            }

            func.hasReturned = false;
            func.returnValue = {TokenType::End, ""};

            std::vector<Token> args;

            if ((func.parent != parent && func.layer > Layer) || func.layer > Layer) {
                throwError("This function is not accessible in the current scope.", t, numLine);
                return pos;
            }

            Token LParen = tokens[pos++];
            if (isConstructor) LParen = tokens[pos++];
            if (LParen.type != TokenType::LParen) {
                throwError("Expected '(' after the function name.", LParen, numLine);
                return pos;
            }

            Token p = tokens[pos++];
            while (p.type != TokenType::LParen &&
                    p.type != TokenType::RParen &&
                    p.type != TokenType::LBrace &&
                    p.type != TokenType::RBrace) {
                Token arg = p;
                if (arg.type != TokenType::Comma) {
                    //std::cout << "DEBUGARG: " << arg.text << "\n";
                    if (isTokenType(arg) && 
                        arg.type != TokenType::Number &&
                        arg.type != TokenType::StringLiteral) {
                        throwError("The variable name is invalid.", arg, numLine);
                        return pos;
                    }
                    args.push_back(arg);
                }
                p = tokens[pos++];
            }
            pos--;

            Token RParen = tokens[pos++]; // )
            if (RParen.type != TokenType::RParen) {
                throwError("Expected ')' to close the function arguments.", RParen, numLine);
                return pos;
            }

            Token semi = tokens[pos++];
            if (semi.type != TokenType::Semicolon && !isRetCall && caller == "print") {
                throwError("Expected ';' after the function call.", semi, numLine);
                return pos;
            }

            if (args.size() != func.params.size()) {
                throwError("The function was called with the wrong number of arguments.", {TokenType::End, func.name}, numLine);
                return pos;
            }

            for (size_t i = 0; i < args.size(); i++) {
                Token arg = args[i];
                bool isConst = false;
                std::string varStr = "";
                long long varNum = 0;

                if (!isVarName(arg) && 
                    arg.type != TokenType::StringLiteral && 
                    arg.type != TokenType::Number) {
                    throwError("One or more arguments are invalid or undefined.", args[i], numLine);
                    return pos;

                } else if (arg.type == TokenType::Identifier || isVarName(arg)) {
                    auto& var = vars[arg.text];
                    if ((var.parent != parent && var.layer > Layer) || var.layer > Layer) {
                        throwError("This variable is not accessible in the current scope.", arg, numLine);
                        return pos;
                    }
                    isConst = var.isConst;

                    if (var.isString) varStr = var.str;
                    else varNum = var.number;
                } else if (arg.type == TokenType::StringLiteral) {
                    varStr = arg.text;
                } else if (arg.type == TokenType::Number) {
                    varNum = std::stoll(arg.text);
                }

                if (!isClassFunc)
                    func.localVars[func.params[i].text] = Value::handleVal(
                                                                    func.name, 
                                                                    newLayer, 
                                                                    isConst, 
                                                                    varStr, 
                                                                    varNum);
                else if (isClassFunc)
                    classes[parent].variables[func.params[i].text].value = Value::handleVal(
                                                                    func.name, 
                                                                    newLayer, 
                                                                    isConst, 
                                                                    varStr, 
                                                                    varNum, 
                                                                    {TokenType::Empty, parent});
            }

            run(func.body, func.name, newLayer, numLine);

            Token ret = func.returnValue;
            return pos;
        }












        struct Arithmetic {
            Token val;
            size_t newPos;
        };

        Arithmetic getReturn(
            Token t, 
            const int Layer, 
            std::string parent, 
            int numLine, 
            std::vector<Token> tokens,
            int pos,
            std::string caller = "root",
            bool isRetCall = false
        ) {
            size_t newPos = pos;
            if (isFuncName(t)) {
                newPos = executeFunction(t, Layer, parent, numLine, tokens, pos, caller, isRetCall);

                auto& func = funcs[t.text];
                if (func.hasReturned) {
                    return {func.returnValue, newPos};
                } else {
                    throwError("The function has not returned a value yet.", t, numLine);
                    return {Token{TokenType::End, ""}, newPos};
                }
            } else {
                throwError("The function does not exist or is not accessible in the current scope.", t, numLine);
                return {Token{TokenType::End, ""}, newPos};
            }
        }

        void throwError(std::string error, Token token, int numLine) {
            std::string errorMsg = error 
                                + " '" + token.text
                                + "' (Line " 
                                + std::to_string(numLine) + ")"
                                + "\n";
            throw std::runtime_error(errorMsg);
        }

        Token handleArithmetic(const std::vector<Token>& operations) {
            std::vector<Token> input = operations;
            
            while (true)
            {
                int depth = 0;
                bool changed = false;

                for (size_t i = 0; i < input.size(); i++)
                {
                    if (input[i].type == TokenType::LParen)
                    {
                        depth++;
                    }
                    else if (input[i].type == TokenType::RParen)
                    {
                        depth--;

                        if (depth == 0)
                        {
                            std::vector<Token> inner(
                                input.begin() + 1,
                                input.begin() + i
                            );

                            Token result = handleArithmetic(inner);

                            input.erase(input.begin(), input.begin() + i + 1);
                            input.insert(input.begin(), result);

                            changed = true;
                            break;
                        }
                    }
                }

                if (!changed)
                    break;
            }

            std::vector<Token> expr;

            for (const auto& t : input)
            {
                if (t.type == TokenType::LParen || t.type == TokenType::RParen)
                    continue;

                expr.push_back(t);
            }

            if (expr.size() == 1)
                return expr.front();



            if (expr.empty())
                return { End, "" };

            if (expr.size() == 1)
                return expr.front();

            auto precedence = [](TokenType t) {
                switch (t) {
                    case Multiply:
                    case Divide:
                    case Modulo:
                        return 3;

                    case Plus:
                    case Minus:
                        return 2;

                    case Less:
                    case Greater:
                    case LessEqual:
                    case GreaterEqual:
                    case Equal:
                    case NotEqual:
                        return 1;

                    case And:
                        return 0;

                    case Or:
                        return 0;

                    default:
                        return -1;
                }
            };

            auto apply = [](Token lhs, Token op, Token rhs) -> Token {
                Token result;

                auto toNumber = [](const Token& t) -> long long {
                    return std::stoll(t.text);
                };

                // STRING handling nur bei +
                if (op.type == Plus &&
                    (lhs.type == StringLiteral || rhs.type == StringLiteral))
                {
                    Token l = lhs;
                    Token r = rhs;

                    if (l.type != StringLiteral)
                        l.text = std::to_string(std::stoll(l.text));

                    if (r.type != StringLiteral)
                        r.text = std::to_string(std::stoll(r.text));

                    return { StringLiteral, l.text + r.text };
                }

                // BOOL operators
                if (op.type == Equal ||
                    op.type == NotEqual ||
                    op.type == Less ||
                    op.type == Greater ||
                    op.type == LessEqual ||
                    op.type == GreaterEqual)
                {
                    long long a = toNumber(lhs);
                    long long b = toNumber(rhs);

                    bool r = false;

                    switch (op.type)
                    {
                        case Equal:        r = (a == b); break;
                        case NotEqual:     r = (a != b); break;
                        case Less:         r = (a < b); break;
                        case Greater:      r = (a > b); break;
                        case LessEqual:    r = (a <= b); break;
                        case GreaterEqual: r = (a >= b); break;
                        default: break;
                    }

                    return { Number, r ? "1" : "0" };
                }

                // AND / OR
                if (op.type == And || op.type == Or)
                {
                    long long a = toNumber(lhs);
                    long long b = toNumber(rhs);

                    bool r = false;

                    if (op.type == And)
                        r = (a != 0 && b != 0);
                    else
                        r = (a != 0 || b != 0);

                    return { Number, r ? "1" : "0" };
                }

                // Normal math
                long long a = 0;
                long long b = 0;

                try {
                    a = toNumber(lhs);
                    b = toNumber(rhs);
                }
                catch (...)
                {
                    throw std::runtime_error("This operation requires numeric values.");
                }

                result.type = Number;

                switch (op.type)
                {
                    case Plus:
                        result.text = std::to_string(a + b);
                        break;

                    case Minus:
                        result.text = std::to_string(a - b);
                        break;

                    case Multiply:
                        result.text = std::to_string(a * b);
                        break;

                    case Divide:
                        if (b == 0)
                            throw std::runtime_error("Division by zero is not allowed.");

                        result.text = std::to_string(a / b);
                        break;

                    case Modulo:
                        if (b == 0)
                            throw std::runtime_error("Modulo by zero is not allowed.");

                        result.text = std::to_string(a % b);
                        break;

                    default:
                        throw std::runtime_error("The operator is not supported.");
                }

                return result;
            };

            std::vector<Token> values;
            std::vector<Token> ops;

            for (const auto& t : expr) {
                if (t.type == Number || t.type == StringLiteral) {
                    values.push_back(t);
                } else if (
                    t.type == Plus ||
                    t.type == Minus ||
                    t.type == Multiply ||
                    t.type == Divide ||
                    t.type == Modulo ||
                    t.type == Equal ||
                    t.type == NotEqual ||
                    t.type == Less ||
                    t.type == Greater ||
                    t.type == LessEqual ||
                    t.type == GreaterEqual ||
                    t.type == And ||
                    t.type == Or)
                {
                    while (!ops.empty() &&
                        precedence(ops.back().type) >= precedence(t.type))
                    {
                        if (values.size() < 2 || ops.empty())
                            throw std::runtime_error("Invalid expression structure.");

                        Token rhs = values.back(); values.pop_back();
                        Token lhs = values.back(); values.pop_back();
                        Token op  = ops.back(); ops.pop_back();

                        values.push_back(apply(lhs, op, rhs));
                    }

                    ops.push_back(t);
                } else {
                    throw std::runtime_error("Invalid token in expression: " + t.text);
                }
            }

            while (!ops.empty())
            {
                if (values.size() < 2)
                    return values.back();

                Token rhs = values.back(); values.pop_back();
                Token lhs = values.back(); values.pop_back();
                Token op  = ops.back(); ops.pop_back();

                values.push_back(apply(lhs, op, rhs));
            }

            if (values.empty())
                return { End, "" };

            return values.back();
        }

        Arithmetic initilaizeArithmetic(
            std::vector<Token> tokens, 
            size_t pos, 
            std::string parent, 
            int Layer, 
            int numLine, 
            Token className = {TokenType::Empty, "root"},
            Token varName = {TokenType::Empty, ""}
        ) {
            std::vector<Token> operations;

            Token val = tokens[pos++];
            while (val.type != TokenType::Semicolon) {
                Token operation = val;
                //std::cout << "DEBUGN: " << val.text << "\n";
                //std::cout << "DEBUGP: " << parent << "\n";
                //std::cout << "DEBUGC: " << className.text << "\n";
                //std::cout << "DEBUGV: " << val.text << "\n";
                //std::cout << "DEBUGV: " << varName.text << "\n";
                //std::cout << "DEBUG?: " << (isClassVariable(className, val) ? "ClassVar" : "NoClassVar") << "\n";
                //std::cout << "DEBUG?: " << (isClassVariable({TokenType::Empty, parent}, val) ? "ClassVar" : "NoClassVar") << "\n";
                if (isVarName(val) || 
                    isLocalVarName(parent, val) || 
                    isFuncName(val) || 
                    isClassVariable({TokenType::Empty, parent}, val) || 
                    isClassVariable(className, val)
                ) {
                    Value var;
                    if (isVarName(val))
                        var = vars[val.text];
                    else if (isLocalVarName(parent, val))
                        var = funcs[parent].localVars[val.text];
                    else if (isFuncName(val)) {
                        Arithmetic ret = getReturn(val, Layer, parent, numLine, tokens, pos, "arithmetic", true);
                        var = tokenToValue(ret.val);
                        pos = ret.newPos;
                        pos--;
                    } else if (isClassVariable({TokenType::Empty, parent}, val)) {
                        var = classes[parent].variables[val.text].value;
                    } else if (isClassVariable(className, val)) {
                        var = classes[className.text].variables[val.text].value;
                    }

                    if (var.isString) {
                        operation = {TokenType::StringLiteral, var.str};
                    } else {
                        operation = {TokenType::Number, std::to_string(var.number)};
                    }
                }
                operations.push_back(operation);
                val = tokens[pos++];
            }
            val = handleArithmetic(operations);
            pos--;

            return {val, pos};
        }

        Token resolveToken(const Token& t, const std::string& parent) {
            if (t.type == TokenType::Identifier) {

                if (isVarName(t)) {
                    auto& var = vars[t.text];

                    if (var.isString)
                        return {TokenType::StringLiteral, var.str};

                    return {TokenType::Number, std::to_string(var.number)};
                } else if (isLocalVarName(parent, t)) {
                    auto& var = funcs[parent].localVars[t.text];

                    if (var.isString)
                        return {TokenType::StringLiteral, var.str};

                    return {TokenType::Number, std::to_string(var.number)};
                } else 
                    throw std::runtime_error("Unknown variable in condition: " + t.text);
            }

            return t;
        }

        bool evaluateCondition(std::string parent, std::vector<Token> expr) {
            // Variablen auflösen
            for (Token &t : expr)
                t = resolveToken(t, parent);

            // Äußere Klammern entfernen
            while (expr.size() >= 2 &&
                expr.front().type == TokenType::LParen &&
                expr.back().type == TokenType::RParen)
            {
                int depth = 0;
                bool remove = true;

                for (size_t i = 0; i < expr.size(); i++)
                {
                    if (expr[i].type == TokenType::LParen)
                        depth++;

                    if (expr[i].type == TokenType::RParen)
                        depth--;

                    if (depth == 0 && i != expr.size() - 1)
                    {
                        remove = false;
                        break;
                    }
                }

                if (!remove)
                    break;

                expr.erase(expr.begin());
                expr.pop_back();
            }

            // OR
            {
                int depth = 0;

                for (size_t i = 0; i < expr.size(); i++)
                {
                    if (expr[i].type == TokenType::LParen) depth++;
                    else if (expr[i].type == TokenType::RParen) depth--;

                    if (depth == 0 && expr[i].type == TokenType::Or)
                    {
                        std::vector<Token> left(expr.begin(), expr.begin() + i);
                        std::vector<Token> right(expr.begin() + i + 1, expr.end());

                        return evaluateCondition(parent, left) ||
                            evaluateCondition(parent, right);
                    }
                }
            }

            // AND
            {
                int depth = 0;

                for (size_t i = 0; i < expr.size(); i++)
                {
                    if (expr[i].type == TokenType::LParen) depth++;
                    else if (expr[i].type == TokenType::RParen) depth--;

                    if (depth == 0 && expr[i].type == TokenType::And)
                    {
                        std::vector<Token> left(expr.begin(), expr.begin() + i);
                        std::vector<Token> right(expr.begin() + i + 1, expr.end());

                        return evaluateCondition(parent, left) &&
                            evaluateCondition(parent, right);
                    }
                }
            }

            // Vergleichsoperator suchen
            int depth = 0;

            for (size_t i = 0; i < expr.size(); i++)
            {
                if (expr[i].type == TokenType::LParen)
                    depth++;

                else if (expr[i].type == TokenType::RParen)
                    depth--;

                if (depth != 0)
                    continue;

                TokenType op = expr[i].type;

                if (op == TokenType::Equal ||
                    op == TokenType::NotEqual ||
                    op == TokenType::Less ||
                    op == TokenType::Greater ||
                    op == TokenType::LessEqual ||
                    op == TokenType::GreaterEqual)
                {
                    std::vector<Token> left(expr.begin(), expr.begin() + i);
                    std::vector<Token> right(expr.begin() + i + 1, expr.end());

                    Token lhs = handleArithmetic(left);
                    Token rhs = handleArithmetic(right);

                    if (lhs.type == TokenType::StringLiteral ||
                        rhs.type == TokenType::StringLiteral)
                    {
                        switch (op)
                        {
                            case TokenType::Equal:
                                return lhs.text == rhs.text;

                            case TokenType::NotEqual:
                                return lhs.text != rhs.text;

                            default:
                                throw std::runtime_error("Only == and != are allowed for strings.");
                        }
                    }

                    long long a = std::stoll(lhs.text);
                    long long b = std::stoll(rhs.text);

                    switch (op)
                    {
                        case TokenType::Equal:        return a == b;
                        case TokenType::NotEqual:     return a != b;
                        case TokenType::Less:         return a < b;
                        case TokenType::Greater:      return a > b;
                        case TokenType::LessEqual:    return a <= b;
                        case TokenType::GreaterEqual: return a >= b;
                        default: break;
                    }
                }
            }

            Token result = handleArithmetic(expr);

            if (result.type == TokenType::StringLiteral)
                return !result.text.empty();

            return std::stoll(result.text) != 0;
        }

        bool evaluateFlatCondition(std::string parent, const std::vector<Token>& expr) {
            auto toInt = [](const std::string& s) -> int {
                if (s.empty())
                    throw std::runtime_error("Empty value in condition");

                for (char c : s)
                    if (!std::isdigit(c) && c != '-')
                        throw std::runtime_error("Non-numeric value in condition: " + s);

                return std::stoi(s);
            };

            if (expr.size() == 1)
                return toInt(expr[0].text) != 0;

            for (size_t i = 1; i + 1 < expr.size(); i++) {
                std::vector<int> nums = {0, 0};
                const Token& op = expr[i];

                (void) nums; (void)parent;

                int left = toInt(expr[i - 1].text);
                int right = toInt(expr[i + 1].text);

                switch (op.type) {
                    case TokenType::Equal:          return left == right;
                    case TokenType::NotEqual:       return left != right;
                    case TokenType::Less:           return left < right;
                    case TokenType::Greater:        return left > right;
                    case TokenType::LessEqual:      return left <= right;
                    case TokenType::GreaterEqual:   return left >= right;
                    case TokenType::And:            return left && right;
                    case TokenType::Or:             return left || right;
                    default:
                        break;
                }
            }

            throw std::runtime_error("Invalid condition expression.");
        }

        ConditionResult initializeCondition(
            const std::vector<Token>& tokens,
            size_t pos,
            const std::string& parent)
        {
            std::vector<Token> expression;
            int parenDepth = 1;

            while (pos < tokens.size()) {
                Token t = tokens[pos++];

                if (t.type == TokenType::LParen)
                    parenDepth++;

                if (t.type == TokenType::RParen) {
                    parenDepth--;

                    if (parenDepth == 0)
                        break;
                }

                if (isVarName(t)) {
                    auto& var = vars[t.text];

                    if (var.isString)
                        expression.push_back({TokenType::StringLiteral, var.str});
                    else
                        expression.push_back({TokenType::Number, std::to_string(var.number)});
                } else if (isLocalVarName(parent, t)) {
                    auto& var = funcs[parent].localVars[t.text];

                    if (var.isString)
                        expression.push_back({TokenType::StringLiteral, var.str});
                    else
                        expression.push_back({TokenType::Number, std::to_string(var.number)});
                } else {
                    expression.push_back(t);
                }
            }

            bool result = evaluateCondition(parent, expression);

            return { result, pos };
        }














        size_t registerVar(
            std::vector<Token> tokens, 
            size_t pos, 
            const int Layer, 
            std::string parent, 
            int numLine,
            Token t,
            Token classOutName = {TokenType::Empty, "root"}
        ) {
            bool isConst = false;
            std::string par = parent;
            int lay = Layer;
            Token name;
            std::string varStr = "";
            long long varNum = 0;

            if (!isVarName(t) && !isLocalVarName(parent, t) && 
                !isClassVariable({TokenType::Empty, parent}, t)) {
                if (t.type == TokenType::Const)
                    isConst = true;
                name = tokens[pos++];
                
                if (isTokenType(name)) {
                    throwError("The variable name is invalid.", combine({tokens[pos - 2], tokens[pos - 1], tokens[pos]}), numLine);
                    return pos;
                } 
            } else if ((isVarName(t) && vars[t.text].isConst) || 
                        (isLocalVarName(parent, t) && funcs[parent].localVars[t.text].isConst)) {
                throwError("Cannot modify the value of a constant.", combine({t, name}), numLine);
                return pos;
            } else if (isClassVariable({TokenType::Empty, parent}, t) && 
                        t.type != TokenType::Let && t.type != TokenType::Const) {
                Token dot = tokens[pos++];
                if (dot.type != TokenType::Dot) {
                    throwError("Expected '.' after the class name.", dot, numLine);
                    return pos;
                }

                if (isClassVariable({TokenType::Empty, parent}, t)) {
                    name = t;
                    par = t.text;
                    lay = Layer;
                } else {
                    throwError("Expected a class Variable after the class name.", tokens[pos], numLine);
                    return pos;
                }
            } else {
                name = t;
                if (isVarName(t)) {
                    par = vars[name.text].parent;
                    lay = vars[name.text].layer;
                } else if (isLocalVarName(parent, t)) {
                    par = funcs[parent].localVars[t.text].parent;
                    lay = funcs[parent].localVars[t.text].layer;
                } else if (isClassName({TokenType::Empty, parent})) {
                    par = classes[parent].variables[t.text].value.parent;
                    lay = classes[parent].variables[t.text].value.layer;
                }
            }

            if (name.type == TokenType::Let || name.type == TokenType::Const)
                name = tokens[pos++];

            //std::cout << "DEBUG1: " << t.text << "\n";
            //std::cout << "DEBUG2: " << name.text << "\n";
            Token op = tokens[pos++]; // =

            if (op.type == TokenType::Assign) {
                if (tokens[pos].type == TokenType::New) {
                    pos++; // new
                    Token className = tokens[pos++];

                    if (!isClassName(className)) {
                        throwError("Unknown class.", className, numLine);
                        return pos;
                    }

                    if ((isVarName(name) || !isVarName(name)) && !isLocalVarName(parent, name)) {
                        vars[name.text] = Value::handleVal(
                            par,
                            lay,
                            isConst,
                            varStr,
                            varNum,
                            className);
                    } else if (isLocalVarName(parent, name)) {
                        funcs[parent].localVars[name.text] = Value::handleVal(
                            par,
                            lay,
                            isConst,
                            varStr,
                            varNum,
                            className);
                    } 

                    pos = executeFunction(
                        tokens[pos - 1], 
                        Layer + 1, 
                        className.text, 
                        numLine, 
                        tokens, 
                        pos - 1, 
                        "constructor", 
                        false, 
                        true
                    );

                    return pos;
                } else {
                    Arithmetic arithmetic = initilaizeArithmetic(tokens, pos, parent, Layer, numLine, classOutName, name);
                    Token val = arithmetic.val;
                    pos = arithmetic.newPos--;

                    if (val.type == TokenType::Number)
                        varNum = std::stoll(val.text);
                    else if (val.type == TokenType::StringLiteral) 
                        varStr = val.text;
                }

            } else if (op.type == TokenType::And || op.type == TokenType::Or ||
                       op.type == TokenType::Equal || op.type == TokenType::NotEqual ||
                       op.type == TokenType::Less || op.type == TokenType::Greater ||
                       op.type == TokenType::LessEqual || op.type == TokenType::GreaterEqual) {
                std::vector<Token> condition;
                condition.push_back(name);
                condition.push_back(op);

                while (pos < tokens.size()) {
                    Token next = tokens[pos++];
                    if (next.type == TokenType::Semicolon)
                        break;
                    condition.push_back(next);
                }

                bool result = evaluateCondition(parent, condition);
                varStr = result ? "True" : "False";
            } else if (op.type == TokenType::Plus ||
                        op.type == TokenType::Minus) {
                Token next = tokens[pos++];
                if (next.type == op.type) {
                    Value var;
                    std::string opName;
                    std::string operation;

                    if (isVarName(t))
                        var = vars[name.text];
                    else if (isLocalVarName(parent, t))
                        var = funcs[parent].localVars[t.text];

                    if (next.type == TokenType::Plus) {
                        opName = "Increment";
                        operation = "++";
                    } else if (next.type == TokenType::Minus) {
                        opName = "Decrement";
                        operation = "--";
                    }

                    if (var.isString) {
                        throwError("Cannot " + opName + " a string value using '" + operation + "'.", name, numLine);
                        return pos;
                    }

                    long long val = var.number;
                    isConst = var.isConst;

                    if (op.type == TokenType::Plus)
                        varNum = ++val;
                    else if (op.type == TokenType::Minus)
                        varNum = --val;
                } else {
                    throwError("Expected '++' or '--'. Both operators must match.", combine({op, next}), numLine);
                    return pos;
                }
            } else if ( op.type == TokenType::Plus || 
                        op.type == TokenType::Minus ||
                        op.type == TokenType::Multiply || 
                        op.type == TokenType::Divide ||
                        op.type == TokenType::Modulo) {
                Value var;

                Token equal = tokens[pos++];
                if (equal.type != TokenType::Assign) {
                    throwError("Expected '=' after the operator.", equal, numLine);
                    return pos;
                }

                if (isVarName(t))
                    var = vars[name.text];
                else if (isLocalVarName(parent, t))
                    var = funcs[parent].localVars[t.text];
                else {
                    throwError("The variable is not defined.", name, numLine);
                    return pos;
                }

                if (var.isString) {
                    throwError("Cannot perform arithmetic operations on a string value.", name, numLine);
                    return pos;
                }

                Arithmetic arithmetic = initilaizeArithmetic(tokens, pos, parent, Layer, numLine, classOutName, name);
                pos = arithmetic.newPos--;

                if (op.type == TokenType::Plus) {
                    varNum = var.number + std::stoll(arithmetic.val.text);
                } else if (op.type == TokenType::Minus) {
                    varNum = var.number - std::stoll(arithmetic.val.text);
                } else if (op.type == TokenType::Multiply) {
                    varNum = var.number * std::stoll(arithmetic.val.text);
                } else if (op.type == TokenType::Divide) {
                    if (std::stoll(arithmetic.val.text) == 0) {
                        throwError("Division by zero is not allowed.", arithmetic.val, numLine);
                        return pos;
                    }
                    varNum = var.number / std::stoll(arithmetic.val.text);
                } else if (op.type == TokenType::Modulo) {
                    if (std::stoll(arithmetic.val.text) == 0) {
                        throwError("Modulo by zero is not allowed.", arithmetic.val, numLine);
                        return pos;
                    }
                    varNum = var.number % std::stoll(arithmetic.val.text);
                }
            } else {
                throwError("Expected '=' or '++' or '--' after the variable name.", op, numLine);
                return pos;
            }
            
            if (!isClassName({TokenType::Empty, parent})) {
                if ((isVarName(name) || !isVarName(name)) && !isLocalVarName(parent, name)) {
                    vars[name.text] = Value::handleVal(
                        par,
                        lay,
                        isConst,
                        varStr,
                        varNum);
                } else if (isLocalVarName(parent, name)) {
                    funcs[parent].localVars[name.text] = Value::handleVal(
                        par,
                        lay,
                        isConst,
                        varStr,
                        varNum);
                } 
            } else if (isClassName({TokenType::Empty, parent})) {
                classes[parent].variables[name.text] = {
                    Value::handleVal(par, lay, isConst, varStr, varNum),
                    AccessModifier::Private
                };
            } else {
                throwError("This variable couldn't be saved", name, numLine);
                return pos;
            }
                
    
            Token semi = tokens[pos++];
            if (semi.type != TokenType::Semicolon) {
                throwError("Unexpected Character6", semi, numLine);
                return pos;
            }
            return pos;
        }




        size_t registerFunc(
            std::vector<Token> tokens, 
            size_t pos, 
            const int Layer, 
            std::string parent, 
            int numLine,
            bool isConstructor = false
        ) {

            //Check if Function Name is valid
            Token name;
            if (isConstructor) name = {TokenType::Empty, parent};
            else  name = tokens[pos++];

            if (isTokenType(name) && !isConstructor) {
                throwError("The function name is invalid.", name, numLine);
                return pos;
            } else if (isFuncName(name)) {
                throwError("A function with this name already exists.", name, numLine);
                return pos;
            }

            std::vector<Token> params = {};
            std::vector<Token> funcBody;

            //Check if this Function has a (
            Token LParen = tokens[pos++]; // (
            if (LParen.type != TokenType::LParen) {
                throwError("Expected '(' after the function name.", LParen, numLine);
                return pos;
            }

            //Iterate over all the Parameters in the ()
            Token p = tokens[pos++];
            while (p.type != TokenType::LParen &&
                    p.type != TokenType::RParen &&
                    p.type != TokenType::LBrace &&
                    p.type != TokenType::RBrace) {
                Token arg = p;
                if (arg.type != TokenType::Comma) {
                    //std::cout << "DEBUGARG: " << arg.text << "\n";
                    if (isTokenType(arg)) {
                        throwError("This variable name is invalid", arg, numLine);
                        return pos;
                    }
                    auto param = arg;
                    params.push_back(param);
                }
                p = tokens[pos++];
            }
            pos--;
            Token RParen = tokens[pos++]; // )
            if (RParen.type != TokenType::RParen) {
                throwError("Expected ')' to close the function parameter list.", RParen, numLine);
                return pos;
            }


            //Check if Function has a {
            Token lBrace = tokens[pos++]; // {
            if (lBrace.type != TokenType::LBrace) {
                throwError("Expected '{' to begin the function body.", lBrace, numLine);
                return pos;
            }

            //Iterate over Function Body 
            int braceDepth = 1;
            while (braceDepth > 0) {
                Token b = tokens[pos++];

                if (b.type == TokenType::LBrace) {
                    braceDepth++;
                }
                else if (b.type == TokenType::RBrace) {
                    braceDepth--;
                    if (braceDepth == 0)
                        break;
                }

                funcBody.push_back(b);
            }

            //Register function in the unordered map 
            if (isConstructor)
                classes[parent].functions["constructor"] = {
                    {Layer, parent, name.text, params, funcBody},
                    AccessModifier::Public
                };
            else if (isClassName({TokenType::Empty, parent}))
                classes[parent].functions[name.text] = {
                    {Layer, parent, name.text, params, funcBody},
                    AccessModifier::Private
                };
            else if (parent == "root" || isFuncName({TokenType::Empty, parent}))
                funcs[name.text] = {
                    Layer,
                    parent,
                    name.text,
                    params,
                    funcBody
                };
            //Function is now ready to use

            return pos;
        }
















        

    public:
        std::vector<std::string> geistScriptCode;
        void execute(const std::string& source) {
            Lexer lex(source);

            std::vector<Token> tokens;
            while (true) {
                Token t = lex.next();
                tokens.push_back(t);
                if (t.type == TokenType::End)
                    break;
            }

            executeTokens(tokens);
        }

        void executeTokens(const std::vector<Token>& tokens, std::string parent = "root", int layer = 0, int numLine = 0) {
            const int Layer = layer;
            size_t pos = 0;

            std::function<void(const std::vector<Token>&, std::string, int, int)> run;
            run = [&](const std::vector<Token>& toks, std::string p, int l, int numLine) {
                executeTokens(toks, p, l, numLine);
            };

            while (pos < tokens.size()) {
                numLine++;
                Token t = tokens[pos++];
                if (t.type == TokenType::End)
                    break;

                /*if (t.type == TokenType::Minus) {
                    Token next = tokens[pos++];

                    if (next.type == TokenType::NewLine) {
                        numLine++;
                        //std::cout << "DEBUG: Line " << numLine << " | ";
                    }
                }*/

                if (parent != "root" && funcs[parent].hasReturned)
                    return;

                if (isFuncName(t)) {
                    pos = executeFunction(t, Layer, parent, numLine, tokens, pos);
                }

                else if (isClassName(vars[t.text].className)) {
                    Token clsName = vars[t.text].className;
                    Token dot = tokens[pos++];
                    if (dot.type != TokenType::Dot) {
                        throwError("Expected '.' after the class name.", dot, numLine);
                        break;
                    }

                    Token func = tokens[pos];
                    if (isClassFunction(clsName, func)) {
                        pos = executeFunction(clsName, layer + 1, clsName.text, numLine, tokens, pos);
                    } else {
                        throwError("Expected a class function after the class name.", tokens[pos], numLine);
                        break;
                    }
                }

                else if (t.type == TokenType::Return) {
                    Arithmetic arithmetic = initilaizeArithmetic(tokens, pos, parent, layer, numLine);

                    funcs[parent].returnValue = arithmetic.val;
                    funcs[parent].hasReturned = true;

                    return;
                }
    
                else if (t.type == TokenType::Print) {
                    Token LParen = tokens[pos++]; // (
                    if (LParen.type != TokenType::LParen) {
                        throwError("Expected '(' at the beginning of a print statement.", LParen, numLine);
                        break;
                    }

                    Arithmetic arithmetic = initilaizeArithmetic(tokens, pos, parent, Layer, numLine);
                    Token v = arithmetic.val;
                    pos = arithmetic.newPos--;
    
                    if (v.type == TokenType::StringLiteral)
                        std::cout << v.text;
                    else if (v.type == TokenType::Number)
                        std::cout << v.text;
                    else if (v.type == TokenType::Identifier) {
                        Value var;
                        if (isVarName(v) && !isLocalVarName(parent, v)) {
                            var = vars[v.text];
                        } else if (isLocalVarName(parent, v)) {
                            auto& func = funcs[parent];
                            var = func.localVars[v.text];
                        } else if (isFuncName(v)) {
                            Arithmetic ret = getReturn(v, Layer, parent, numLine, tokens, pos, "print", true);
                            var = tokenToValue(ret.val);
                            pos = ret.newPos;
                        }

                        if ((var.parent != parent && var.layer > Layer) || var.layer > Layer) {
                            throwError("This variable is not accessible in the current scope.", v, numLine);
                            break;
                        }
                        if (var.isString) std::cout << var.str;
                        else std::cout << var.number;
                    }
                    pos--;
    
                    Token RParen = tokens[pos++]; // )
                    if (RParen.type != TokenType::RParen) {
                        throwError("Expected ')' to close the print statement.", RParen, numLine);
                        break;
                    }

                    Token semi = tokens[pos++]; // ;
                    if (semi.type != TokenType::Semicolon) {
                        throwError("Expected ';' after the print statement.", semi, numLine);
                        break;
                    }
                    std::cout << "\n";
                }

                else if (t.type == TokenType::If) {
                    Token lp = tokens[pos++];
                    if (lp.type != TokenType::LParen) {
                        throwError("Expected '('.", lp, numLine);
                        break;
                    }

                    std::vector<Token> condition;
                    int parenDepth = 1;

                    while (pos < tokens.size() && parenDepth > 0) {
                        Token tok = tokens[pos++];

                        if (tok.type == TokenType::LParen)
                            parenDepth++;

                        if (tok.type == TokenType::RParen) {
                            parenDepth--;

                            if (parenDepth == 0)
                                break;
                        }

                        condition.push_back(tok);
                    }

                    Token lb = tokens[pos++];
                    if (lb.type != TokenType::LBrace)
                        throwError("Expected '{'.", lb, numLine);

                    std::vector<Token> body;
                    int braceDepth = 1;

                    while (pos < tokens.size() && braceDepth > 0) {
                        Token tok = tokens[pos++];

                        if (tok.type == TokenType::LBrace)
                            braceDepth++;

                        if (tok.type == TokenType::RBrace) {
                            braceDepth--;

                            if (braceDepth == 0)
                                break;
                        }

                        body.push_back(tok);
                    }

                    bool executed = false;

                    if (evaluateCondition(parent, condition)) {
                        executeTokens(body, parent, Layer + 1, numLine);
                        executed = true;
                    }

                    while (pos < tokens.size()) {
                        Token next = tokens[pos];

                        if (next.type != TokenType::Else)
                            break;

                        pos++; // consume 'else'

                        bool isElseIf = false;
                        (void) isElseIf;

                        if (pos < tokens.size() && tokens[pos].type == TokenType::If) {
                            isElseIf = true;
                            pos++; // consume 'if'

                            Token lp2 = tokens[pos++];
                            if (lp2.type != TokenType::LParen)
                                throwError("Expected '('.", lp2, numLine);

                            std::vector<Token> cond2;
                            int depth2 = 1;

                            while (pos < tokens.size() && depth2 > 0) {
                                Token tok = tokens[pos++];

                                if (tok.type == TokenType::LParen)
                                    depth2++;

                                if (tok.type == TokenType::RParen) {
                                    depth2--;
                                    if (depth2 == 0)
                                        break;
                                }

                                cond2.push_back(tok);
                            }

                            Token lb2 = tokens[pos++];
                            if (lb2.type != TokenType::LBrace)
                                throwError("Expected '{'.", lb2, numLine);

                            std::vector<Token> body2;
                            int brace2 = 1;

                            while (pos < tokens.size() && brace2 > 0) {
                                Token tok = tokens[pos++];

                                if (tok.type == TokenType::LBrace)
                                    brace2++;

                                if (tok.type == TokenType::RBrace) {
                                    brace2--;
                                    if (brace2 == 0)
                                        break;
                                }

                                body2.push_back(tok);
                            }

                            if (!executed && evaluateCondition(parent, cond2)) {
                                executeTokens(body2, parent, Layer + 1, numLine);
                                executed = true;
                            }
                        }
                        else {
                            // ELSE block
                            Token lb3 = tokens[pos++];
                            if (lb3.type != TokenType::LBrace)
                                throwError("Expected '{'.", lb3, numLine);

                            std::vector<Token> body3;
                            int brace3 = 1;

                            while (pos < tokens.size() && brace3 > 0) {
                                Token tok = tokens[pos++];

                                if (tok.type == TokenType::LBrace)
                                    brace3++;

                                if (tok.type == TokenType::RBrace) {
                                    brace3--;
                                    if (brace3 == 0)
                                        break;
                                }

                                body3.push_back(tok);
                            }

                            if (!executed) {
                                executeTokens(body3, parent, Layer + 1, numLine);
                                executed = true;
                            }

                            break;
                        }
                    }
                }

                else if (t.type == TokenType::While) {
                    Token lp = tokens[pos++];
                    if (lp.type != TokenType::LParen)
                        throwError("Expected '('.", lp, numLine);

                    size_t condStart = pos;
                    (void) condStart;
                    int depth = 1;

                    std::vector<Token> condExpr;

                    while (pos < tokens.size()) {
                        Token x = tokens[pos++];

                        if (x.type == TokenType::LParen)
                            depth++;

                        if (x.type == TokenType::RParen) {
                            depth--;
                            if (depth == 0)
                                break;
                        }

                        condExpr.push_back(x);
                    }

                    Token rp = tokens[pos - 1];

                    Token lb = tokens[pos++];
                    if (lb.type != TokenType::LBrace)
                        throwError("Expected '{'.", lb, numLine);

                    std::vector<Token> body;
                    int braceDepth = 1;

                    while (pos < tokens.size()) {
                        Token x = tokens[pos++];

                        if (x.type == TokenType::LBrace)
                            braceDepth++;

                        if (x.type == TokenType::RBrace) {
                            braceDepth--;
                            if (braceDepth == 0)
                                break;
                        }

                        body.push_back(x);
                    }

                    while (true) {
                        bool cond = evaluateCondition(parent, condExpr);

                        if (!cond)
                            break;

                        executeTokens(body, parent, Layer + 1, numLine);
                    }
                }

                else if (t.type == TokenType::For) {
                    Token lp = tokens[pos++];
                    if (lp.type != TokenType::LParen)
                        throwError("Expected '('.", lp, numLine);

                    std::vector<Token> init;
                    std::vector<Token> cond;
                    std::vector<Token> update;
                    int part = 0;

                    while (pos < tokens.size()) {
                        Token x = tokens[pos++];

                        if (x.type == TokenType::RParen)
                            break;

                        if (x.type == TokenType::Semicolon) {
                            part++;
                            continue;
                        }

                        if (part == 0) init.push_back(x);
                        else if (part == 1) cond.push_back(x);
                        else if (part == 2) update.push_back(x);
                    }

                    if (pos >= tokens.size())
                        throwError("Unexpected end in for-loop header.", tokens[pos], numLine);

                    Token lb = tokens[pos++];
                    if (lb.type != TokenType::LBrace)
                        throwError("Expected '{'.", lb, numLine);

                    std::vector<Token> body;
                    int depth = 1;

                    while (pos < tokens.size() && depth > 0) {
                        Token x = tokens[pos++];

                        if (x.type == TokenType::LBrace) depth++;
                        if (x.type == TokenType::RBrace) depth--;

                        if (depth > 0)
                            body.push_back(x);
                    }

                    if (depth != 0)
                        throwError("Missing closing '}' in for-loop.", tokens[pos], numLine);

                    
                    init.push_back({TokenType::Semicolon, ";"});
                    init.push_back({TokenType::End, ""});

                    update.push_back({TokenType::Semicolon, ";"});
                    update.push_back({TokenType::End, ""});

                    body.push_back({TokenType::End, ""});

                    executeTokens(init, parent, Layer + 1, numLine);

                    while (evaluateCondition(parent, cond)) {
                        executeTokens(body, parent, Layer + 1, numLine);
                        executeTokens(update, parent, Layer + 1, numLine);
                    }
                }

                else if (t.type == TokenType::Function) {
                    pos = registerFunc(tokens, pos, Layer, parent, numLine);
                }

                else if (t.type == TokenType::NewClass) {
                    Token name = tokens[pos++];
                    if (isTokenType(name)) {
                        throwError("The class name is invalid.", name, numLine);
                        break;
                    } else if (classes.find(name.text) != classes.end()) {
                        throwError("A class with this name already exists.", name, numLine);
                        break;
                    }

                    std::vector<Token> classBody;

                    classes[name.text] = {
                        name.text,
                        parent,
                        layer + 1,
                    };

                    Token lBrace = tokens[pos++];
                    if (lBrace.type != TokenType::LBrace) {
                        throwError("Expected '{' to begin the class body.", lBrace, numLine);
                        break;
                    }

                    int braceDepth = 1;
                    while (braceDepth > 0) {
                        size_t tmpPos = pos;
                        tmpPos++;
                        Token b = tokens[pos++];
                        Token innerName = tokens[tmpPos++];

                        if (b.type == TokenType::LBrace) {
                            braceDepth++;
                        }
                        else if (b.type == TokenType::RBrace) {
                            braceDepth--;
                            if (braceDepth == 0)
                                break;
                        }

                        else if (b.text == name.text) {
                            pos = registerFunc(tokens, pos, layer + 1, name.text, numLine, true);
                        }

                        else if (b.type == TokenType::Function) {
                            pos = registerFunc(tokens, pos, layer + 1, name.text, numLine);
                        } 

                        else if (b.type == TokenType::Let || 
                                b.type == TokenType::Const || 
                                isVarName(b)) {
                            std::cout << "CLASS: " << name.text << "\n";
                            pos = registerVar(tokens, pos, layer + 1, name.text, numLine, b, name);
                        }
                    }
                }

                else if ((t.type == TokenType::Let || 
                        t.type == TokenType::Const ||
                        isVarName(t)) && 
                        !isFuncName(t) &&
                        t.type != TokenType::Function) {
                    //std::cout << "DEBUGT:" << t.text << "\n";
                    pos = registerVar(tokens, pos, Layer, parent, numLine, t);
                }

                else {
                    if (!isTokenType(t))
                        throwError("This statement or expression is not recognized.", t, numLine);
                }
            }
        }

        std::string getFileContent() {
            std::vector<std::string> lines;
            std::string result;

            return result;
        }

        void downloadExample() {
            std::string fileName = "script.gsScript";
            std::ofstream outfile (fileName);
            outfile << "my text here!" << std::endl;
            outfile.close();
            std::cout << "Example script created: " << fileName << "\n";
        }
    };
}




void cmd_script(const std::vector<std::string>& args, Terminal& term) {
    (void) term;
    if (args.size() > 2) {
        help.printHelp("script", {"file"}, false, "", true);
        return;
    }

    std::string type = "script";
    std::string scriptExt = "gsScript";
    std::string errorCol = getAnsiColor('C');

    GeistScript::Interpreter interp;
    std::string script;
    int startIndex = 1;
    int numLine = 0;

    if (args[1] == "--example" || args[1] == "-e") {
        interp.downloadExample();
        return;
    }

    for (size_t i = startIndex; i < args.size(); i++) {
        std::string fileName = args[i];
        std::ifstream file(fileName);

        if (fileName.substr(fileName.find_last_of(".") + 1) != scriptExt) {
            std::cerr   << errorCol 
                        << type << ": Invalid file type for (" 
                        << fileName << "). Expected ." 
                        << scriptExt << "\n" 
                        << currentColor;
            continue;
        }
        if (!file.is_open()) {
            std::cerr   << errorCol 
                        << type << ": Cannot open " 
                        << fileName 
                        << " filename is invalid or file does not exist."
                        << "\n" 
                        << currentColor;
            continue;
        }

        std::string line;
        int lineNumber = 1;
        (void)lineNumber;

        while (std::getline(file, line)) {
            interp.geistScriptCode.push_back(line);
            script += line + "\n";
            lineNumber++;
            numLine++;
        }

        file.close();
    }

    interp.execute(script);
}









struct Permissions {
    bool read = false;
    bool write = false;
    bool exec = false;
    bool sudo = false;
};

struct Command {
    std::string name;
    std::function<std::string(const std::vector<std::string>&, const std::string&)> func;
    Permissions perms;
};

class CommandManager {
public:
    CommandManager(Terminal& term) : terminal(term) {
        initCommands();
    }

    void registerAll() {
        for (const auto& cmd : commands) {
            terminal.registerCommand(
                cmd.name,
                cmd.func,
                cmd.perms.read,
                cmd.perms.write,
                cmd.perms.exec,
                cmd.perms.sudo
            );
        }
    }

private:
    Terminal& terminal;
    std::vector<Command> commands;

    void initCommands() {

        commands = {

            {"help", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_help(args);
                return "";
            }, 
            {false, false, true, false}},

            {"clear", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_clear(args);
                return "";
            },
            {false, false, true, false}},

            {"echo", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_echo(args);
                return "";
            }, 
            {false, false, true, false}},

            {"ls", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_ls(args);
                return "";
            }, 
            {true, false, true, false}},

            {"exit", [this](const auto& args, const std::string& input){
                (void)args; (void)input;
                std::cout << currentColor << "Stopping Terminal...\n";
                terminal.stop();
                return "";
            }, 
            {false, false, true, false}},

            {"ping", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_ping(args);
                return "";
            }, 
            {false, false, true, false}},

            {"dir", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_dir(args);
                return "";
            }, 
            {true, false, true, false}},

            {"cd", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_cd(args);
                return "";
            }, 
            {true, false, true, false}},

            {"mkdir", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_mkdir(args);
                return "";
            }, 
            {false, true, true, false}},

            {"rm", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_rm(args);
                return "";
            }, 
            {true, false, true, false}},

            {"touch", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_touch(args);
                return "";
            }, 
            {true, true, true, false}},

            {"vim", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_vim(args);
                return "";
            }, 
            {true, true, true, false}},

            {"apt", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_apt_install(args);
                return "";
            }, 
            {true, true, true, true}},

            {"color", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_color(args);
                return "";
            }, 
            {false, false, true, false}},

            {"passwd", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_passwd(args, terminal);
                return "";
            }, 
            {true, true, true, true}},

            {"perm", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_perm(args, terminal);
                return "";
            }, 
            {true, true, true, true}},

            {"user", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_user(args, terminal);
                return "";
            }, 
            {true, true, true, true}},

            {"print", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_print(args, terminal);
                return "";
            }, 
            {true, true, true, false}},

            {"win", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_win(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"date", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_date(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"sys", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_sys(args, terminal);
                return "";
            }, 
            {true, false, true, false}},

            {"games", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_games(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"bank", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_bank(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"graph", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_graph(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"cat", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_cat(args, terminal);
                return "";
            }, 
            {true, false, true, false}},

            {"db", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_db(args, terminal);
                return "";
            }, 
            {false, false, true, false}}, 

            {"man", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_man(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"draw", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_draw(args, terminal);
                return "";
            }, 
            {false, false, true, false}},

            {"script", [this](const auto& args, const std::string& input){
                (void)input;
                cmd_script(args, terminal);
                return "";
            }, 
            {false, false, true, false}}
        };
    }
};


// ==========================
// main
// ==========================
int main() {
    std::ios::sync_with_stdio(false);
    setvbuf(stdout, nullptr, _IONBF, 0);

    Terminal terminal;

    CommandManager manager(terminal);
    manager.registerAll();

    // normale Befehle
    /*terminal.registerCommand("help", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_help(args);
            return "";
        }, false, false, false
    );

    terminal.registerCommand("clear", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_clear(args);
            return "";
        }, false, false, false
    );

    terminal.registerCommand("echo", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_echo(args);
            return "";
        }, false, false, false
    );

    terminal.registerCommand("ls", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_ls(args);
            return "";
        }, false, false, false
    );

    terminal.registerCommand("exit",
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input; (void)args; (void)input;
            std::cout << currentColor + "Stopping Terminal...\n";
            terminal.stop();
            return "";
        }
    );

    terminal.registerCommand("logout", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input; (void)args;
            while (!terminal.loginPrompt()) {}
            terminal.run();
            return "";
        }
    );

    terminal.registerCommand("ping", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_ping(args);
            return "";
        }, false, false, true
    );

    terminal.registerCommand("dir", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_dir(args);
            return "";
        }, true
    );

    terminal.registerCommand("cd", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_cd(args);
            return "";
        }, true, false, false
    );

    terminal.registerCommand("mkdir", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_mkdir(args);
            return "";
        }, false, true
    );

    terminal.registerCommand("rm", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_rm(args);
            return "";
        }, true
    );

    terminal.registerCommand("touch", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_touch(args);
            return "";
        }, true, true
    );

    terminal.registerCommand("vim", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_vim(args);
            return "";
        }, true, true
    );

    terminal.registerCommand("apt", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_apt_install(args);
            return "";
        }, true, true, false, true
    );

    //color changer
    terminal.registerCommand("color", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_color(args);
            return "";
        }
    );

    terminal.registerCommand("passwd", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_passwd(args, terminal);
            return "";
        }, true, true, true, true
    );

    // permission management commands (sudo required)
    terminal.registerCommand("perm", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_perm(args, terminal);
            return "";
        }, true, true, true, true
    );

    // user management commands (sudo required)
    terminal.registerCommand("user", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_user(args, terminal);
            return "";
        }, 
        true, true, true, true
    );
    // Read Write Execute Sudo

    terminal.registerCommand("print", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_print(args, terminal);
            return "";
        }, 
        true, true
    );

    terminal.registerCommand("win", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_win(args, terminal);
            return "";
        }
    );

    terminal.registerCommand("date", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_date(args, terminal);
            return "";
        }
    );

    terminal.registerCommand("sys", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_sys(args, terminal);
            return "";
        }
    );

    terminal.registerCommand("games", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_games(args, terminal);
            return "";
        }
    );

    terminal.registerCommand("bank", 
        [&](const std::vector<std::string>& args, const std::string& input) -> std::string {
            (void)input;
            cmd_bank(args, terminal);
            return "";
        }
    );*/

    terminal.currentUser = &terminal.users["root"];

    std::string input;

    terminal.run(input);

    return 0;
}