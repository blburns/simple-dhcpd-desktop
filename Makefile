# Makefile for SimpleDhcpdDesktop Application
# .NET 9 Avalonia UI Desktop Application

# Project Configuration
PROJECT_NAME := SimpleDhcpdDesktop.App
PROJECT_PATH := SimpleDhcpdDesktop.App
SOLUTION_FILE := DesktopBoilerplate.sln
TARGET_FRAMEWORK := net9.0

# Build Configurations
CONFIG_DEBUG := Debug
CONFIG_RELEASE := Release

# Runtime Identifiers
RID_WIN_X64 := win-x64
RID_WIN_X86 := win-x86
RID_OSX_X64 := osx-x64
RID_OSX_ARM64 := osx-arm64
RID_LINUX_X64 := linux-x64
RID_LINUX_ARM64 := linux-arm64

# Default values
CONFIG ?= $(CONFIG_DEBUG)
SELF_CONTAINED ?= false
RUNTIME ?= 

# Colors for output (if supported)
ifneq ($(TERM),)
	COLOR_RESET := \033[0m
	COLOR_BOLD := \033[1m
	COLOR_GREEN := \033[32m
	COLOR_YELLOW := \033[33m
	COLOR_BLUE := \033[34m
	COLOR_CYAN := \033[36m
endif

.PHONY: help
help: ## Show this help message
	@echo "$(COLOR_BOLD)SimpleDhcpdDesktop Build System$(COLOR_RESET)"
	@echo ""
	@echo "$(COLOR_BOLD)Usage:$(COLOR_RESET)"
	@echo "  make [target] [options]"
	@echo ""
	@echo "$(COLOR_BOLD)Common Targets:$(COLOR_RESET)"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  $(COLOR_CYAN)%-20s$(COLOR_RESET) %s\n", $$1, $$2}'
	@echo ""
	@echo "$(COLOR_BOLD)Examples:$(COLOR_RESET)"
	@echo "  make build                    # Build Debug configuration"
	@echo "  make build CONFIG=Release     # Build Release configuration"
	@echo "  make run                      # Run the application"
	@echo "  make watch                    # Run with hot reload"
	@echo "  make publish-win              # Publish for Windows x64"
	@echo "  make publish-osx              # Publish for macOS (current arch)"
	@echo "  make publish-all              # Publish for all platforms"

.PHONY: check-dotnet
check-dotnet: ## Check if .NET SDK is installed
	@echo "$(COLOR_BLUE)Checking .NET SDK...$(COLOR_RESET)"
	@dotnet --version || (echo "$(COLOR_YELLOW)Error: .NET SDK not found. Please install .NET 9 SDK.$(COLOR_RESET)" && exit 1)
	@echo "$(COLOR_GREEN).NET SDK found$(COLOR_RESET)"

.PHONY: restore
restore: check-dotnet ## Restore NuGet packages
	@echo "$(COLOR_BLUE)Restoring NuGet packages...$(COLOR_RESET)"
	dotnet restore $(SOLUTION_FILE)
	@echo "$(COLOR_GREEN)Packages restored$(COLOR_RESET)"

.PHONY: build
build: restore ## Build the project (default: Debug)
	@echo "$(COLOR_BLUE)Building $(CONFIG) configuration...$(COLOR_RESET)"
	dotnet build $(PROJECT_PATH) -c $(CONFIG) --no-restore
	@echo "$(COLOR_GREEN)Build completed$(COLOR_RESET)"

.PHONY: build-debug
build-debug: CONFIG := $(CONFIG_DEBUG)
build-debug: build ## Build Debug configuration

.PHONY: build-release
build-release: CONFIG := $(CONFIG_RELEASE)
build-release: build ## Build Release configuration

.PHONY: clean
clean: ## Clean build artifacts
	@echo "$(COLOR_BLUE)Cleaning build artifacts...$(COLOR_RESET)"
	dotnet clean $(PROJECT_PATH) -c $(CONFIG)
	@echo "$(COLOR_GREEN)Clean completed$(COLOR_RESET)"

.PHONY: clean-all
clean-all: ## Clean all build artifacts (Debug and Release)
	@echo "$(COLOR_BLUE)Cleaning all build artifacts...$(COLOR_RESET)"
	dotnet clean $(SOLUTION_FILE)
	@echo "$(COLOR_GREEN)Clean all completed$(COLOR_RESET)"

.PHONY: rebuild
rebuild: clean build ## Clean and rebuild

.PHONY: run
run: build ## Run the application
	@echo "$(COLOR_BLUE)Running application...$(COLOR_RESET)"
	dotnet run --project $(PROJECT_PATH) -c $(CONFIG) --no-build

.PHONY: watch
watch: ## Run with hot reload (for development)
	@echo "$(COLOR_BLUE)Running with hot reload...$(COLOR_RESET)"
	@echo "$(COLOR_YELLOW)Press Ctrl+C to stop$(COLOR_RESET)"
	dotnet watch --project $(PROJECT_PATH)

.PHONY: publish
publish: restore ## Publish the application (set RUNTIME and SELF_CONTAINED)
	@if [ -z "$(RUNTIME)" ]; then \
		echo "$(COLOR_YELLOW)Error: RUNTIME must be set. Use publish-win, publish-osx, or publish-linux$(COLOR_RESET)"; \
		exit 1; \
	fi
	@echo "$(COLOR_BLUE)Publishing for $(RUNTIME) (self-contained: $(SELF_CONTAINED))...$(COLOR_RESET)"
	dotnet publish $(PROJECT_PATH) -c $(CONFIG_RELEASE) -r $(RUNTIME) --self-contained $(SELF_CONTAINED)
	@echo "$(COLOR_GREEN)Published to: $(PROJECT_PATH)/bin/$(CONFIG_RELEASE)/$(TARGET_FRAMEWORK)/$(RUNTIME)/publish/$(COLOR_RESET)"

# Windows Publishing Targets
.PHONY: publish-win
publish-win: RUNTIME := $(RID_WIN_X64)
publish-win: SELF_CONTAINED := false
publish-win: publish ## Publish for Windows x64 (framework-dependent)

.PHONY: publish-win-x64
publish-win-x64: RUNTIME := $(RID_WIN_X64)
publish-win-x64: SELF_CONTAINED := true
publish-win-x64: publish ## Publish for Windows x64 (self-contained)

.PHONY: publish-win-x86
publish-win-x86: RUNTIME := $(RID_WIN_X86)
publish-win-x86: SELF_CONTAINED := true
publish-win-x86: publish ## Publish for Windows x86 (self-contained)

# macOS Publishing Targets
.PHONY: publish-osx
publish-osx: ## Publish for macOS (detects architecture)
	@if [ "$$(uname -m)" = "arm64" ]; then \
		$(MAKE) publish-osx-arm64; \
	else \
		$(MAKE) publish-osx-x64; \
	fi

.PHONY: publish-osx-x64
publish-osx-x64: RUNTIME := $(RID_OSX_X64)
publish-osx-x64: SELF_CONTAINED := true
publish-osx-x64: publish ## Publish for macOS Intel (x64)

.PHONY: publish-osx-arm64
publish-osx-arm64: RUNTIME := $(RID_OSX_ARM64)
publish-osx-arm64: SELF_CONTAINED := true
publish-osx-arm64: publish ## Publish for macOS Apple Silicon (ARM64)

.PHONY: publish-osx-universal
publish-osx-universal: ## Publish for macOS Universal (both architectures)
	@echo "$(COLOR_BLUE)Publishing for macOS Universal...$(COLOR_RESET)"
	$(MAKE) publish-osx-x64
	$(MAKE) publish-osx-arm64
	@echo "$(COLOR_GREEN)Universal build completed$(COLOR_RESET)"

# Linux Publishing Targets
.PHONY: publish-linux
publish-linux: RUNTIME := $(RID_LINUX_X64)
publish-linux: SELF_CONTAINED := true
publish-linux: publish ## Publish for Linux x64

.PHONY: publish-linux-x64
publish-linux-x64: RUNTIME := $(RID_LINUX_X64)
publish-linux-x64: SELF_CONTAINED := true
publish-linux-x64: publish ## Publish for Linux x64 (self-contained)

.PHONY: publish-linux-arm64
publish-linux-arm64: RUNTIME := $(RID_LINUX_ARM64)
publish-linux-arm64: SELF_CONTAINED := true
publish-linux-arm64: publish ## Publish for Linux ARM64 (self-contained)

# Publish All Platforms
.PHONY: publish-all
publish-all: ## Publish for all supported platforms
	@echo "$(COLOR_BLUE)Publishing for all platforms...$(COLOR_RESET)"
	@echo "$(COLOR_YELLOW)This may take a while...$(COLOR_RESET)"
	$(MAKE) publish-win-x64
	$(MAKE) publish-win-x86
	$(MAKE) publish-osx-x64
	$(MAKE) publish-osx-arm64
	$(MAKE) publish-linux-x64
	$(MAKE) publish-linux-arm64
	@echo "$(COLOR_GREEN)All platforms published$(COLOR_RESET)"

# Development Targets
.PHONY: dev
dev: watch ## Alias for watch (development mode)

.PHONY: format
format: ## Format code using dotnet format
	@echo "$(COLOR_BLUE)Formatting code...$(COLOR_RESET)"
	dotnet format $(SOLUTION_FILE)
	@echo "$(COLOR_GREEN)Formatting completed$(COLOR_RESET)"

.PHONY: test
test: ## Run tests (if test projects exist)
	@echo "$(COLOR_BLUE)Running tests...$(COLOR_RESET)"
	@if dotnet test $(SOLUTION_FILE) 2>/dev/null; then \
		echo "$(COLOR_GREEN)Tests completed$(COLOR_RESET)"; \
	else \
		echo "$(COLOR_YELLOW)No test projects found$(COLOR_RESET)"; \
	fi

# Information Targets
.PHONY: info
info: ## Show project information
	@echo "$(COLOR_BOLD)Project Information:$(COLOR_RESET)"
	@echo "  Project: $(PROJECT_NAME)"
	@echo "  Solution: $(SOLUTION_FILE)"
	@echo "  Target Framework: $(TARGET_FRAMEWORK)"
	@echo ""
	@echo "$(COLOR_BOLD).NET SDK:$(COLOR_RESET)"
	@dotnet --version || echo "  Not installed"
	@echo ""
	@echo "$(COLOR_BOLD)Supported Runtimes:$(COLOR_RESET)"
	@echo "  Windows: $(RID_WIN_X64), $(RID_WIN_X86)"
	@echo "  macOS: $(RID_OSX_X64), $(RID_OSX_ARM64)"
	@echo "  Linux: $(RID_LINUX_X64), $(RID_LINUX_ARM64)"

.PHONY: list-publishes
list-publishes: ## List all published outputs
	@echo "$(COLOR_BOLD)Published Outputs:$(COLOR_RESET)"
	@find $(PROJECT_PATH)/bin/$(CONFIG_RELEASE) -type d -name "publish" 2>/dev/null | while read dir; do \
		runtime=$$(echo $$dir | sed 's|.*/\([^/]*\)/publish|\1|'); \
		size=$$(du -sh "$$dir" 2>/dev/null | cut -f1); \
		echo "  $$runtime: $$dir ($$size)"; \
	done || echo "  No published outputs found"

# Default target
.DEFAULT_GOAL := help
