#pragma once
#include <tuple>
#include <vector>
#include <string>
#include <sstream>
#include <iomanip>
#include <iostream>

template<typename... Ts>
class VariadicTable
{
public:
    using RowType = std::tuple<Ts...>;

    VariadicTable(const std::vector<std::string>& headers, size_t cellPadding = 1)
        : _headers(headers), _cellPadding(cellPadding)
    {
        if (sizeof...(Ts) != headers.size()) {
            throw std::runtime_error(
                "Header count must match number of template parameters");
        }
    }

    void addRow(const RowType& row)
    {
        _data.push_back(row);
    }

    void print(std::ostream& stream)
    {
        computeColumnSizes();

        const std::string lineColor  = "\033[1;30m";
        const std::string headerColor = "\033[36m";
        const std::string dataColor   = "\033[1;36m";
        const std::string reset = "\033[0m";

        size_t totalWidth = _headers.size() + 1;
        for (size_t w : _columnSizes)
            totalWidth += w + (2 * _cellPadding);

        // Top line
        stream << lineColor << std::string(totalWidth, '-') << reset << "\n";

        // Header
        stream << lineColor << "|" << reset;
        for (size_t i = 0; i < _headers.size(); i++)
        {
            stream << headerColor
                   << std::string(_cellPadding, ' ')
                   << std::setw(_columnSizes[i]) << std::left << _headers[i]
                   << std::string(_cellPadding, ' ')
                   << reset;

            stream << lineColor << "|" << reset;
        }
        stream << "\n";

        // Header bottom line
        stream << lineColor << std::string(totalWidth, '-') << reset << "\n";

        // Data rows
        for (auto& row : _data)
        {
            stream << lineColor << "|" << reset;
            printTuple(stream, row, dataColor, lineColor, reset);
            stream << "\n";
        }

        // Last line
        stream << lineColor << std::string(totalWidth, '-') << reset << "\n";
    }

private:
    // ------------------------------------------
    // COLUMN SIZE CALCULATION
    // ------------------------------------------

    void computeColumnSizes()
    {
        _columnSizes.clear();
        _columnSizes.resize(sizeof...(Ts));

        // header sizes
        for (size_t i = 0; i < _headers.size(); i++)
            _columnSizes[i] = _headers[i].size();

        // data sizes
        for (auto& row : _data)
            sizeTuple(row);
    }

    template<size_t I = 0>
    typename std::enable_if<I == sizeof...(Ts), void>::type
    sizeTuple(const RowType&) {}

    template<size_t I = 0>
    typename std::enable_if<I < sizeof...(Ts), void>::type
    sizeTuple(const RowType& row)
    {
        std::stringstream ss;
        ss << std::get<I>(row);
        _columnSizes[I] = std::max(_columnSizes[I], ss.str().size());
        sizeTuple<I + 1>(row);
    }

    // ------------------------------------------
    // TUPLE PRINTING
    // ------------------------------------------

    template<size_t I = 0>
    typename std::enable_if<I == sizeof...(Ts), void>::type
    printTuple(std::ostream&, const RowType&,
               const std::string&, const std::string&, const std::string&) {}

    template<size_t I = 0>
    typename std::enable_if<I < sizeof...(Ts), void>::type
    printTuple(std::ostream& stream, const RowType& row,
               const std::string& dataColor,
               const std::string& lineColor,
               const std::string& reset)
    {
        std::stringstream ss;
        ss << std::get<I>(row);

        stream << dataColor
               << std::string(_cellPadding, ' ')
               << std::setw(_columnSizes[I]) << std::left << ss.str()
               << std::string(_cellPadding, ' ')
               << reset;

        stream << lineColor << "|" << reset;

        printTuple<I + 1>(stream, row, dataColor, lineColor, reset);
    }

private:
    std::vector<std::string> _headers;
    size_t _cellPadding;

    std::vector<size_t> _columnSizes;
    std::vector<RowType> _data;
};

