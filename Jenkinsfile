pipeline {
    agent any
    environment {
        myVersion = '0.9'
    }
    tools {
        msbuild '.NET Core 2.2.0'
    }
    stages {
        stage('restore') {
            steps {
                sh 'dotnet restore'
            }
        }
        stage('build') {
            steps {
                sh 'dotnet build'
            }
        }
        stage('publish') {
            steps {

            }
        }
    }
}