#include <iostream>
#include <string>
#include <filesystem>
#include <fstream>
#include <sstream>
#include <regex>
#include <vector>
#include <map>
#include <chrono>
#include <cstring>

namespace fs = std::filesystem;

// Color codes for terminal output
class Colors {
public:
    static constexpr const char* RESET = "\033[0m";
    static constexpr const char* BOLD = "\033[1m";
    static constexpr const char* GREEN = "\033[32m";
    static constexpr const char* BLUE = "\033[34m";
    static constexpr const char* YELLOW = "\033[33m";
    static constexpr const char* RED = "\033[31m";
    static constexpr const char* CYAN = "\033[36m";
};

// Utility functions
class Utils {
public:
    static bool isValidProjectName(const std::string& name) {
        std::regex pattern("^[A-Z][a-zA-Z0-9]*$");
        return std::regex_match(name, pattern);
    }

    static std::string toKebabCase(const std::string& input) {
        std::string result;
        for (size_t i = 0; i < input.length(); ++i) {
            if (isupper(input[i]) && i > 0) {
                result += '-';
                result += tolower(input[i]);
            } else {
                result += tolower(input[i]);
            }
        }
        return result;
    }

    static std::string trim(const std::string& str) {
        size_t first = str.find_first_not_of(" \t\n\r");
        if (first == std::string::npos) return "";
        size_t last = str.find_last_not_of(" \t\n\r");
        return str.substr(first, (last - first + 1));
    }

    static bool pathExists(const fs::path& path) {
        return fs::exists(path);
    }

    static bool isDirectory(const fs::path& path) {
        return fs::is_directory(path);
    }

    static void printHeader() {
        std::cout << "\n";
        std::cout << Colors::BOLD << Colors::BLUE
                  << "╔════════════════════════════════════════════════════════════╗\n"
                  << "║   Enterprise CQRS Microservice Generator - CLI             ║\n"
                  << "║   Standalone Executable with Embedded Template             ║\n"
                  << "╚════════════════════════════════════════════════════════════╝\n"
                  << Colors::RESET << "\n";
    }

    static void printUsage(const std::string& programName) {
        std::cout << Colors::CYAN << "USAGE:\n" << Colors::RESET
                  << "  " << programName << " -n <ServiceName> -o <OutputPath> [OPTIONS]\n\n"
                  << Colors::CYAN << "REQUIRED:\n" << Colors::RESET
                  << "  -n, --name <ServiceName>       Service name (PascalCase, e.g., InvoiceService)\n"
                  << "  -o, --output <Path>            Output directory where service will be created\n\n"
                  << Colors::CYAN << "OPTIONS:\n" << Colors::RESET
                  << "  -c, --company <CompanyName>    Company name for namespace (default: Enterprise)\n"
                  << "  -h, --help                     Show this help message\n"
                  << "  -v, --version                  Show version information\n\n"
                  << Colors::CYAN << "EXAMPLES:\n" << Colors::RESET
                  << "  " << programName << " -n InvoiceService -o C:\\Projects\n"
                  << "  " << programName << " -n OrderService -o /home/user/projects -c AcmeCorp\n"
                  << "  " << programName << " --name PaymentService --output ~/Services --company MyCompany\n\n";
    }

    static void printSuccess(const std::string& message) {
        std::cout << Colors::GREEN << "✓ " << message << Colors::RESET << "\n";
    }

    static void printError(const std::string& message) {
        std::cerr << Colors::RED << "✗ " << message << Colors::RESET << "\n";
    }

    static void printWarning(const std::string& message) {
        std::cout << Colors::YELLOW << "⚠ " << message << Colors::RESET << "\n";
    }

    static void printInfo(const std::string& message) {
        std::cout << Colors::CYAN << "• " << message << Colors::RESET << "\n";
    }

    static void printProgress(const std::string& message) {
        std::cout << Colors::BLUE << "→ " << message << Colors::RESET << "\n";
    }
};

// Template structure for embedded files
struct TemplateFile {
    std::string relativePath;
    std::string content;
    bool isTextFile;
};

// Embedded template files (in real implementation, these would be generated)
std::vector<TemplateFile> getEmbeddedTemplateFiles() {
    // This is a simplified example - in production, generate from actual template
    return {
        {"Template.sln", "[template content]", true},
        {"Template.API/Program.cs", "[template content]", true},
        {"Template.Commands/ICommand.cs", "[template content]", true},
        // ... more files
    };
}

class MicroserviceGenerator {
private:
    std::string serviceName;
    std::string companyName;
    fs::path outputPath;
    fs::path projectPath;

public:
    MicroserviceGenerator(const std::string& name, const std::string& company, const fs::path& outPath)
        : serviceName(name), companyName(company), outputPath(outPath) {
        projectPath = outputPath / serviceName;
    }

    bool validate() {
        Utils::printInfo("Validating inputs...");

        // Validate service name
        if (!Utils::isValidProjectName(serviceName)) {
            Utils::printError("Invalid service name: '" + serviceName + "'");
            Utils::printWarning("Service name must be PascalCase and alphanumeric (e.g., InvoiceService)");
            return false;
        }

        // Validate output path
        if (!Utils::pathExists(outputPath)) {
            Utils::printError("Output directory does not exist: " + outputPath.string());
            return false;
        }

        if (!Utils::isDirectory(outputPath)) {
            Utils::printError("Output path is not a directory: " + outputPath.string());
            return false;
        }

        // Check if project already exists
        if (fs::exists(projectPath)) {
            Utils::printError("Project folder already exists: " + projectPath.string());
            return false;
        }

        Utils::printSuccess("Validation passed");
        return true;
    }

    bool generate() {
        try {
            // Step 1: Extract template
            Utils::printProgress("Extracting embedded template...");
            if (!extractTemplate()) {
                return false;
            }
            Utils::printSuccess("Template extracted");

            // Step 2: Rename files and folders
            Utils::printProgress("Renaming files and folders...");
            if (!renameProjectStructure()) {
                return false;
            }
            Utils::printSuccess("Files and folders renamed");

            // Step 3: Replace text in files
            Utils::printProgress("Updating namespaces and references...");
            if (!replaceTextInFiles()) {
                return false;
            }
            Utils::printSuccess("Code updated with new namespace");

            // Step 4: Generate solution file
            Utils::printProgress("Finalizing project...");
            if (!generateMetaFiles()) {
                return false;
            }
            Utils::printSuccess("Project finalized");

            return true;
        }
        catch (const std::exception& ex) {
            Utils::printError(std::string("Generation failed: ") + ex.what());
            return false;
        }
    }

private:
    bool extractTemplate() {
        try {
            // Create project directory
            fs::create_directories(projectPath);

            // Copy embedded template files to project directory
            auto templateFiles = getEmbeddedTemplateFiles();

            for (const auto& file : templateFiles) {
                fs::path filePath = projectPath / file.relativePath;
                fs::create_directories(filePath.parent_path());

                std::ofstream outFile(filePath);
                if (!outFile) {
                    Utils::printError("Failed to create file: " + filePath.string());
                    return false;
                }

                outFile << file.content;
                outFile.close();
            }

            return true;
        }
        catch (const std::exception& ex) {
            Utils::printError(std::string("Template extraction failed: ") + ex.what());
            return false;
        }
    }

    bool renameProjectStructure() {
        try {
            std::vector<fs::path> toRename;

            // Find all items with "Template" in the name
            for (const auto& entry : fs::recursive_directory_iterator(projectPath)) {
                if (entry.path().filename().string().find("Template") != std::string::npos) {
                    toRename.push_back(entry.path());
                }
            }

            // Rename from deepest to shallowest (reverse order)
            std::sort(toRename.rbegin(), toRename.rend());

            for (const auto& path : toRename) {
                std::string newName = path.filename().string();
                newName = std::regex_replace(newName, std::regex("Template"), serviceName);

                fs::path newPath = path.parent_path() / newName;
                fs::rename(path, newPath);
            }

            return true;
        }
        catch (const std::exception& ex) {
            Utils::printError(std::string("Renaming failed: ") + ex.what());
            return false;
        }
    }

    bool replaceTextInFiles() {
        try {
            std::vector<std::string> extensions = {".cs", ".csproj", ".sln", ".json", ".md", ".xml"};

            for (const auto& entry : fs::recursive_directory_iterator(projectPath)) {
                if (!fs::is_regular_file(entry)) continue;

                std::string ext = entry.path().extension().string();
                if (std::find(extensions.begin(), extensions.end(), ext) == extensions.end()) {
                    continue;
                }

                // Read file
                std::ifstream inFile(entry.path());
                if (!inFile) continue;

                std::stringstream buffer;
                buffer << inFile.rdbuf();
                inFile.close();

                std::string content = buffer.str();
                bool modified = false;

                // Replace Template with ServiceName
                std::string pattern = "\\bTemplate\\b";
                std::string replacement = serviceName;
                std::string newContent = std::regex_replace(content, std::regex(pattern), replacement);

                // Replace lowercase template
                if (newContent != content) modified = true;
                content = newContent;

                // Replace Enterprise with CompanyName if different
                if (companyName != "Enterprise") {
                    pattern = "\\bEnterprise\\b";
                    replacement = companyName;
                    newContent = std::regex_replace(content, std::regex(pattern), replacement);
                    if (newContent != content) modified = true;
                    content = newContent;
                }

                // Write file if modified
                if (modified) {
                    std::ofstream outFile(entry.path());
                    if (!outFile) {
                        Utils::printError("Failed to write file: " + entry.path().string());
                        return false;
                    }
                    outFile << content;
                    outFile.close();
                }
            }

            return true;
        }
        catch (const std::exception& ex) {
            Utils::printError(std::string("Text replacement failed: ") + ex.what());
            return false;
        }
    }

    bool generateMetaFiles() {
        try {
            // Create or update project metadata if needed
            // This could include version files, readme, etc.
            return true;
        }
        catch (const std::exception& ex) {
            Utils::printError(std::string("Meta file generation failed: ") + ex.what());
            return false;
        }
    }
};

// Command-line argument parser
class ArgumentParser {
public:
    struct Arguments {
        std::string serviceName;
        std::string outputPath;
        std::string companyName = "Enterprise";
        bool showHelp = false;
        bool showVersion = false;
        bool isValid = true;
    };

    static Arguments parse(int argc, char* argv[]) {
        Arguments args;

        if (argc < 2) {
            args.isValid = false;
            args.showHelp = true;
            return args;
        }

        for (int i = 1; i < argc; ++i) {
            std::string arg = argv[i];

            if (arg == "-h" || arg == "--help") {
                args.showHelp = true;
                return args;
            }
            else if (arg == "-v" || arg == "--version") {
                args.showVersion = true;
                return args;
            }
            else if ((arg == "-n" || arg == "--name") && i + 1 < argc) {
                args.serviceName = Utils::trim(argv[++i]);
            }
            else if ((arg == "-o" || arg == "--output") && i + 1 < argc) {
                args.outputPath = Utils::trim(argv[++i]);
            }
            else if ((arg == "-c" || arg == "--company") && i + 1 < argc) {
                args.companyName = Utils::trim(argv[++i]);
            }
            else {
                Utils::printError("Unknown argument: " + arg);
                args.isValid = false;
                args.showHelp = true;
                return args;
            }
        }

        // Validate required arguments
        if (args.serviceName.empty() || args.outputPath.empty()) {
            Utils::printError("Missing required arguments");
            args.isValid = false;
            args.showHelp = true;
        }

        return args;
    }
};

int main(int argc, char* argv[]) {
    Utils::printHeader();

    auto args = ArgumentParser::parse(argc, argv);

    if (args.showHelp) {
        Utils::printUsage(argv[0]);
        return args.isValid ? 0 : 1;
    }

    if (args.showVersion) {
        std::cout << "Microservice Generator v1.0.0\n";
        return 0;
    }

    if (!args.isValid) {
        Utils::printUsage(argv[0]);
        return 1;
    }

    // Print configuration
    std::cout << "\n" << Colors::BOLD << "Configuration:" << Colors::RESET << "\n";
    Utils::printInfo("Service Name: " + args.serviceName);
    Utils::printInfo("Company Name: " + args.companyName);
    Utils::printInfo("Output Path: " + args.outputPath);
    std::cout << "\n";

    // Generate microservice
    MicroserviceGenerator generator(args.serviceName, args.companyName, fs::path(args.outputPath));

    if (!generator.validate()) {
        return 1;
    }

    auto start = std::chrono::high_resolution_clock::now();

    if (!generator.generate()) {
        return 1;
    }

    auto end = std::chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start);

    // Success message
    std::cout << "\n" << Colors::BOLD << Colors::GREEN
              << "╔════════════════════════════════════════════════════════════╗\n"
              << "║   ✓ Microservice generated successfully!                  ║\n"
              << "╚════════════════════════════════════════════════════════════╝\n"
              << Colors::RESET;

    Utils::printInfo("Service location: " + (fs::path(args.outputPath) / args.serviceName).string());
    Utils::printInfo("Time elapsed: " + std::to_string(duration.count()) + "ms");

    std::cout << "\n" << Colors::CYAN << "Next steps:" << Colors::RESET << "\n"
              << "  cd " << (fs::path(args.outputPath) / args.serviceName).string() << "\n"
              << "  dotnet build\n"
              << "  dotnet run --project " << args.serviceName << ".API\n\n";

    return 0;
}
