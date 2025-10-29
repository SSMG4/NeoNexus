cmake_minimum_required(VERSION 3.15)
project(NeoNexusNative LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_library(NeoNexusNative SHARED
    NeoNexusNative.cpp
)

target_include_directories(NeoNexusNative PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})

# Export all symbols to simplify P/Invoke (alternative: explicit export)
if (MSVC)
    set_target_properties(NeoNexusNative PROPERTIES
        WINDOWS_EXPORT_ALL_SYMBOLS ON
        RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
    )
endif()
