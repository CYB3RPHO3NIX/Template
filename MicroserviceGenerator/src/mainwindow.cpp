#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "projectgenerator.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QProgressBar>
#include <QFileDialog>
#include <QMessageBox>
#include <QFont>
#include <QDesktopServices>
#include <QUrl>
#include <QRegularExpression>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(std::make_unique<Ui::MainWindow>())
    , generator(std::make_unique<ProjectGenerator>())
{
    ui->setupUi(this);
    setWindowTitle("Enterprise CQRS Microservice Generator");
    setWindowIcon(QIcon(":/icons/app_icon.png"));
    setMinimumSize(600, 500);
    setMaximumSize(800, 700);

    setupUI();
    connectSignals();
}

MainWindow::~MainWindow() = default;

void MainWindow::setupUI()
{
    // Main widget
    auto centralWidget = new QWidget(this);
    setCentralWidget(centralWidget);

    auto mainLayout = new QVBoxLayout(centralWidget);
    mainLayout->setSpacing(15);
    mainLayout->setContentsMargins(20, 20, 20, 20);

    // Title
    auto titleLabel = new QLabel("CQRS Microservice Generator");
    QFont titleFont = titleLabel->font();
    titleFont.setPointSize(16);
    titleFont.setBold(true);
    titleLabel->setFont(titleFont);
    mainLayout->addWidget(titleLabel);

    // Subtitle
    auto subtitleLabel = new QLabel("Generate enterprise microservices from the CQRS template");
    QFont subtitleFont = subtitleLabel->font();
    subtitleFont.setPointSize(10);
    subtitleLabel->setFont(subtitleFont);
    subtitleLabel->setStyleSheet("color: #666;");
    mainLayout->addWidget(subtitleLabel);

    mainLayout->addSpacing(10);

    // Project Name
    auto projectLayout = new QHBoxLayout();
    auto projectLabel = new QLabel("Project Name:");
    projectLabel->setMinimumWidth(120);
    projectLayout->addWidget(projectLabel);
    ui->projectNameInput = new QLineEdit();
    ui->projectNameInput->setPlaceholderText("e.g., InvoiceService");
    projectLayout->addWidget(ui->projectNameInput);
    mainLayout->addLayout(projectLayout);

    // Company Name
    auto companyLayout = new QHBoxLayout();
    auto companyLabel = new QLabel("Company Name:");
    companyLabel->setMinimumWidth(120);
    companyLayout->addWidget(companyLabel);
    ui->companyNameInput = new QLineEdit();
    ui->companyNameInput->setPlaceholderText("e.g., AcmeCorp (optional)");
    ui->companyNameInput->setText("Enterprise");
    companyLayout->addWidget(ui->companyNameInput);
    mainLayout->addLayout(companyLayout);

    // Directory Selection
    auto dirLayout = new QHBoxLayout();
    auto dirLabel = new QLabel("Output Directory:");
    dirLabel->setMinimumWidth(120);
    dirLayout->addWidget(dirLabel);
    ui->directoryInput = new QLineEdit();
    ui->directoryInput->setPlaceholderText("Select output directory...");
    ui->directoryInput->setReadOnly(true);
    dirLayout->addWidget(ui->directoryInput);
    auto browseButton = new QPushButton("Browse...");
    browseButton->setMaximumWidth(100);
    dirLayout->addWidget(browseButton);
    mainLayout->addLayout(dirLayout);

    mainLayout->addSpacing(10);

    // Progress Bar
    ui->progressBar = new QProgressBar();
    ui->progressBar->setVisible(false);
    ui->progressBar->setMaximum(100);
    mainLayout->addWidget(ui->progressBar);

    // Status Message
    ui->statusLabel = new QLabel();
    ui->statusLabel->setWordWrap(true);
    ui->statusLabel->setStyleSheet("color: #333;");
    mainLayout->addWidget(ui->statusLabel);

    mainLayout->addStretch();

    // Buttons
    auto buttonLayout = new QHBoxLayout();
    buttonLayout->addStretch();

    auto createButton = new QPushButton("Generate Microservice");
    createButton->setMinimumWidth(200);
    createButton->setMinimumHeight(40);
    QFont buttonFont = createButton->font();
    buttonFont.setPointSize(11);
    buttonFont.setBold(true);
    createButton->setFont(buttonFont);
    createButton->setStyleSheet(
        "QPushButton {"
        "    background-color: #3B82F6;"
        "    color: white;"
        "    border: none;"
        "    border-radius: 4px;"
        "    padding: 8px 16px;"
        "}"
        "QPushButton:hover {"
        "    background-color: #2563EB;"
        "}"
        "QPushButton:pressed {"
        "    background-color: #1D4ED8;"
        "}"
        "QPushButton:disabled {"
        "    background-color: #BFDBFE;"
        "}"
    );
    ui->createButton = createButton;
    buttonLayout->addWidget(createButton);

    auto exitButton = new QPushButton("Exit");
    exitButton->setMinimumWidth(100);
    exitButton->setMinimumHeight(40);
    exitButton->setFont(buttonFont);
    buttonLayout->addWidget(exitButton);

    mainLayout->addLayout(buttonLayout);

    // Store references
    connect(browseButton, &QPushButton::clicked, this, &MainWindow::onBrowseDirectory);
    connect(ui->createButton, &QPushButton::clicked, this, &MainWindow::onCreateProject);
    connect(exitButton, &QPushButton::clicked, this, &QWidget::close);
    connect(ui->projectNameInput, &QLineEdit::textChanged, this, &MainWindow::onInputChanged);
    connect(ui->directoryInput, &QLineEdit::textChanged, this, &MainWindow::onInputChanged);
}

void MainWindow::connectSignals()
{
    connect(generator.get(), &ProjectGenerator::progressUpdated,
            this, &MainWindow::onGenerationProgress);
    connect(generator.get(), &ProjectGenerator::generationFinished,
            this, &MainWindow::onGenerationFinished);
    connect(generator.get(), &ProjectGenerator::generationError,
            this, &MainWindow::onGenerationError);
}

void MainWindow::onBrowseDirectory()
{
    QString dir = QFileDialog::getExistingDirectory(
        this,
        "Select Output Directory",
        selectedDirectory.isEmpty() ? QDir::homePath() : selectedDirectory,
        QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks
    );

    if (!dir.isEmpty()) {
        selectedDirectory = dir;
        ui->directoryInput->setText(dir);
        updateCreateButtonState();
    }
}

void MainWindow::onCreateProject()
{
    clearMessages();

    if (!validateInputs()) {
        return;
    }

    setUIEnabled(false);
    ui->progressBar->setVisible(true);
    ui->progressBar->setValue(0);

    QString projectName = ui->projectNameInput->text().trimmed();
    QString companyName = ui->companyNameInput->text().trimmed();

    generator->generateProject(projectName, companyName, selectedDirectory);
}

void MainWindow::onGenerationProgress(const QString &message)
{
    ui->statusLabel->setText(message);
    ui->statusLabel->setStyleSheet("color: #0891B2;");
    ui->progressBar->setValue(ui->progressBar->value() + 10);
}

void MainWindow::onGenerationFinished(bool success, const QString &message)
{
    ui->progressBar->setVisible(false);
    setUIEnabled(true);

    if (success) {
        ui->statusLabel->setText(
            "✓ Microservice generated successfully!\n\n" + message
        );
        ui->statusLabel->setStyleSheet("color: #10B981; font-weight: bold;");

        // Show option to open folder
        int result = QMessageBox::information(
            this,
            "Success",
            "Microservice generated successfully!\n\n" + message + "\n\nDo you want to open the folder?",
            QMessageBox::Open | QMessageBox::Close
        );

        if (result == QMessageBox::Open) {
            QString projectPath = selectedDirectory + "/" + ui->projectNameInput->text().trimmed();
            QDesktopServices::openUrl(QUrl::fromLocalFile(projectPath));
        }

        // Clear inputs for next project
        ui->projectNameInput->clear();
    } else {
        ui->statusLabel->setText("✗ " + message);
        ui->statusLabel->setStyleSheet("color: #EF4444; font-weight: bold;");
    }
}

void MainWindow::onGenerationError(const QString &error)
{
    ui->progressBar->setVisible(false);
    setUIEnabled(true);
    ui->statusLabel->setText("✗ Error: " + error);
    ui->statusLabel->setStyleSheet("color: #DC2626; font-weight: bold;");
}

void MainWindow::onInputChanged()
{
    updateCreateButtonState();
}

void MainWindow::validateInputs()
{
    clearMessages();

    QString projectName = ui->projectNameInput->text().trimmed();
    QString directory = selectedDirectory;

    if (projectName.isEmpty()) {
        ui->statusLabel->setText("⚠ Please enter a project name");
        ui->statusLabel->setStyleSheet("color: #F59E0B;");
        return false;
    }

    if (!isValidProjectName(projectName)) {
        ui->statusLabel->setText("⚠ Project name must be PascalCase and alphanumeric (e.g., InvoiceService)");
        ui->statusLabel->setStyleSheet("color: #F59E0B;");
        return false;
    }

    if (directory.isEmpty()) {
        ui->statusLabel->setText("⚠ Please select an output directory");
        ui->statusLabel->setStyleSheet("color: #F59E0B;");
        return false;
    }

    if (!isValidDirectory(directory)) {
        ui->statusLabel->setText("⚠ Selected directory is not valid or accessible");
        ui->statusLabel->setStyleSheet("color: #F59E0B;");
        return false;
    }

    // Check if project already exists
    QDir destDir(directory + "/" + projectName);
    if (destDir.exists()) {
        ui->statusLabel->setText("⚠ Project folder already exists at destination");
        ui->statusLabel->setStyleSheet("color: #F59E0B;");
        return false;
    }

    return true;
}

bool MainWindow::isValidProjectName(const QString &name) const
{
    QRegularExpression regex("^[A-Z][a-zA-Z0-9]*$");
    return regex.match(name).hasMatch();
}

bool MainWindow::isValidDirectory(const QString &path) const
{
    QDir dir(path);
    return dir.exists() && dir.isReadable();
}

void MainWindow::updateCreateButtonState()
{
    bool canCreate = !ui->projectNameInput->text().trimmed().isEmpty()
                     && !selectedDirectory.isEmpty();
    ui->createButton->setEnabled(canCreate);
}

void MainWindow::setUIEnabled(bool enabled)
{
    ui->projectNameInput->setEnabled(enabled);
    ui->companyNameInput->setEnabled(enabled);
    ui->directoryInput->setEnabled(enabled);
    ui->createButton->setEnabled(enabled && !ui->projectNameInput->text().trimmed().isEmpty());
}

void MainWindow::clearMessages()
{
    ui->statusLabel->clear();
}
