cmake_minimum_required(VERSION 3.15)
project(NeoNexusNative LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Collect all .cpp files under this native folder (top-level, src/, win32/, etc.)
file(GLOB_RECURSE NATIVE_SOURCES
    ${CMAKE_CURRENT_SOURCE_DIR}/*.cpp
)

# Exclude any test files if present (adjust patterns if needed)
# list(REMOVE_ITEM NATIVE_SOURCES "${CMAKE_CURRENT_SOURCE_DIR}/tests/SomeTest.cpp")

add_library(NeoNexusNative SHARED ${NATIVE_SOURCES})

# Public include directories for headers
target_include_directories(NeoNexusNative PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    ${CMAKE_CURRENT_SOURCE_DIR}/win32
)

# Export all symbols for simple P/Invoke usage on Windows
if (MSVC)
    set_target_properties(NeoNexusNative PROPERTIES
        WINDOWS_EXPORT_ALL_SYMBOLS ON
    )
endif()

# Put runtime DLL into build/bin so packaging scripts can find it consistently
set_target_properties(NeoNexusNative PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
    RUNTIME_OUTPUT_DIRECTORY_RELEASE "${CMAKE_BINARY_DIR}/bin"
)

# Optionally add warnings / definitions
if (MSVC)
    target_compile_options(NeoNexusNative PRIVATE /W4 /permissive-)
else()
    target_compile_options(NeoNexusNative PRIVATE -Wall -Wextra -Wpedantic)
endif()
