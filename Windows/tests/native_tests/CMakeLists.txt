cmake_minimum_required(VERSION 3.15)
project(NeoNexusNativeTests LANGUAGES CXX)

add_executable(NativeTests NativeTests.cpp)

# Note: Add GoogleTest or other test framework linkage here if installed.
target_compile_features(NativeTests PRIVATE cxx_std_17)
