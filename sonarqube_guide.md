# SonarQube & Testing Guide

This guide explains how to run tests and perform code analysis using SonarQube.

## 1. Running Tests Locally
We have set up xUnit tests for the Application layer.
Run the following command to execute tests and check code coverage:
```powershell
dotnet test --collect:"XPlat Code Coverage"
```
You will find coverage reports in `CA1.UnitTests/TestResults/[GUID]/coverage.cobertura.xml`.

## 2. SonarQube Analysis
For a full analysis, you need a running SonarQube server. The easiest way is via Docker.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed.
- Java Runtime (JRE 17+) installed.
- `dotnet-sonarscanner` tool installed:
  ```powershell
  dotnet tool install --global dotnet-sonarscanner
  ```

### Step 1: Start SonarQube Server
Run the following Docker command:
```powershell
docker run -d --name sonarqube -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true -p 9000:9000 sonarqube:latest
```
- Wait 1-2 minutes for it to start.
- Open `http://localhost:9000`.
- Login with `admin` / `admin`. Change the password when prompted.

### Step 2: Create a Project in SonarQube
1. Click **Create Project** -> **Manually**.
2. Project Key: `CA1_Project`.
3. Display Name: `CA1 Analysis`.
4. Click **Set Up**.
5. Generate a token (e.g., name it "Runner"). **Copy this token**.

### Step 3: Run Analysis
Run these commands in the root `CA1` folder (replace `[YOUR_TOKEN]` with the token you copied):

```powershell
# 1. Begin Analysis
dotnet sonarscanner begin /k:"CA1_Project" /d:sonar.host.url="http://localhost:9000"  /d:sonar.token="[YOUR_TOKEN]" /d:sonar.cs.opencover.reportsPaths="**/coverage.cobertura.xml"

# 2. Build the project
dotnet build

# 3. Run tests again to generate fresh coverage data
dotnet test --collect:"XPlat Code Coverage"

# 4. End Analysis (Uploads to server)
dotnet sonarscanner end /d:sonar.token="[YOUR_TOKEN]"
```

### Static Analysis (Without Server)
We have already installed the `SonarAnalyzer.CSharp` package in this solution.
- This means Visual Studio will automatically highlight SonarQube issues (Code Smells, Bugs) in the **Error List** window as you type, without needing to run the server.
