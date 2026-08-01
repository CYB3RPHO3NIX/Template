#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QString>
#include <QProgressBar>
#include <QLabel>
#include <memory>

namespace Ui {
class MainWindow;
}

class ProjectGenerator;

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private slots:
    void onBrowseDirectory();
    void onCreateProject();
    void onGenerationProgress(const QString &message);
    void onGenerationFinished(bool success, const QString &message);
    void onGenerationError(const QString &error);
    void onInputChanged();

private:
    void setupUI();
    void connectSignals();
    void validateInputs();
    bool isValidProjectName(const QString &name) const;
    bool isValidDirectory(const QString &path) const;
    void updateCreateButtonState();
    void setUIEnabled(bool enabled);
    void clearMessages();

    std::unique_ptr<Ui::MainWindow> ui;
    std::unique_ptr<ProjectGenerator> generator;
    QString selectedDirectory;
};

#endif // MAINWINDOW_H
