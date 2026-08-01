#!/bin/bash

# Microservice Generator - Build Script for Linux/macOS
# Usage: ./build.sh [Release|Debug] [--clean]

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m' # No Color

# Functions
print_success() {
    echo -e "${GREEN}✓ $*${NC}"
}

print_error() {
    echo -e "${RED}✗ $*${NC}" >&2
}

print_info() {
    echo -e "${CYAN}→ $*${NC}"
}

print_warn() {
    echo -e "${YELLOW}⚠ $*${NC}"
}

print_header() {
    echo -e "${BOLD}${BLUE}"
    echo "╔════════════════════════════════════════════════════════════╗"
    echo "║   Microservice Generator - Build Script                   ║"
    echo "╚════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

show_help() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  Release          Release build (default)"
    echo "  Debug            Debug build"
    echo "  --clean          Clean build directory before building"
    echo "  --help           Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0                  # Release build"
    echo "  $0 Debug            # Debug build"
    echo "  $0 --clean          # Clean and rebuild"
    echo "  $0 Debug --clean    # Debug build with clean"
}

# Parse arguments
BUILD_TYPE="Release"
CLEAN_BUILD=false

for arg in "$@"; do
    case "$arg" in
        Release|Debug)
            BUILD_TYPE="$arg"
            ;;
        --clean)
            CLEAN_BUILD=true
            ;;
        --help|-h)
            show_help
            exit 0
            ;;
        *)
            print_error "Unknown option: $arg"
            show_help
            exit 1
            ;;
    esac
done

print_header

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILD_DIR="$SCRIPT_DIR/build"

# Step 1: Check prerequisites
print_info "Checking prerequisites..."

if ! command -v cmake &> /dev/null; then
    print_error "CMake not found. Please install CMake 3.16 or higher"
    echo "  Debian/Ubuntu: sudo apt-get install cmake"
    echo "  macOS: brew install cmake"
    echo "  Or download from: https://cmake.org/download/"
    exit 1
fi

CMAKE_VERSION=$(cmake --version | head -n 1)
print_success "$CMAKE_VERSION"

# Check for C++ compiler
if ! command -v c++ &> /dev/null && ! command -v g++ &> /dev/null && ! command -v clang++ &> /dev/null; then
    print_error "C++ compiler not found"
    echo "  Debian/Ubuntu: sudo apt-get install build-essential"
    echo "  macOS: xcode-select --install"
    exit 1
fi

print_success "C++ compiler found"

# Step 2: Clean if requested
if [ "$CLEAN_BUILD" = true ]; then
    if [ -d "$BUILD_DIR" ]; then
        print_warn "Cleaning build directory..."
        rm -rf "$BUILD_DIR"
    fi
fi

# Step 3: Create build directory
mkdir -p "$BUILD_DIR"

# Step 4: Configure with CMake
print_info "Configuring CMake (BuildType: $BUILD_TYPE)..."

cd "$BUILD_DIR"

if ! cmake -DCMAKE_BUILD_TYPE="$BUILD_TYPE" "$SCRIPT_DIR" > /dev/null 2>&1; then
    print_error "CMake configuration failed"
    exit 1
fi

print_success "CMake configuration successful"

# Step 5: Build
print_info "Building solution..."

if ! cmake --build . --config "$BUILD_TYPE" > /dev/null 2>&1; then
    print_error "Build failed"
    exit 1
fi

print_success "Build successful"

# Step 6: Verify binary
BIN_PATH="$BUILD_DIR/bin/microservice-gen"

echo ""
echo -e "${BOLD}${GREEN}"
echo "╔════════════════════════════════════════════════════════════╗"
echo "║   ✓ Build Completed Successfully!                         ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo -e "${NC}"
echo ""

if [ -f "$BIN_PATH" ]; then
    print_success "Binary location: $BIN_PATH"

    # Get binary size
    if command -v du &> /dev/null; then
        SIZE=$(du -h "$BIN_PATH" | cut -f1)
        print_info "Binary size: $SIZE"
    fi

    # Make executable
    chmod +x "$BIN_PATH"
    print_success "Binary is executable"

    # Test binary
    echo ""
    print_info "Test the executable:"
    echo "  $BIN_PATH --help"
    echo "  $BIN_PATH --version"
    echo "  $BIN_PATH -n TestService -o ./output"
    echo ""

else
    print_warn "Binary not found in expected location"
    echo "  Check build directory: $BUILD_DIR"
fi

print_success "Ready to use!"
