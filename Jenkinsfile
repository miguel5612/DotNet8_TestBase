pipeline {
    agent any

    stages {
        stage('Restore') {
            steps {
                bat 'dotnet restore FrameworkBase.Automation.sln'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build FrameworkBase.Automation.sln --configuration Release --no-restore -m:1'
            }
        }

        stage('API Tests') {
            steps {
                bat 'dotnet test tests\\FrameworkBase.Automation.Api.Tests\\FrameworkBase.Automation.Api.Tests.csproj --configuration Release --no-build --filter "Category=Api"'
            }
        }

        stage('Web Tests') {
            steps {
                bat 'dotnet test tests\\FrameworkBase.Automation.Web.Tests\\FrameworkBase.Automation.Web.Tests.csproj --configuration Release --no-build --filter "Category=Web"'
            }
        }

        stage('Desktop Tests') {
            steps {
                bat 'dotnet test tests\\FrameworkBase.Automation.Desktop.Tests\\FrameworkBase.Automation.Desktop.Tests.csproj --configuration Release --no-build --filter "Category=Desktop"'
            }
        }
    }
}
