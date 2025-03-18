pipeline {
    agent any

    environment {
        SONARQUBE_URL = "http://172.31.26.79:80"
        SONARQUBE_TOKEN = credentials('jenkinsintegration')
        GIT_REPO = "https://github.com/keremyilmaz2/CafeMenuProject.git"
        PROJECT_KEY = "CafeMenuProject"
        DOTNET_ROOT = '/usr/share/dotnet'  
        PATH = "$PATH:/usr/share/dotnet:/root/.dotnet/tools"
        DockerHub_Credentials = credentials('docker-cred')
    }

    stages {
        stage('Login DockerHub') {
            steps {
                sh 'echo $DockerHub_Credentials_PSW | docker login -u $DockerHub_Credentials_USR --password-stdin'
            }
        }
        stage('Checkout Source Code') {
            steps {
                echo "🔹 GitHub'dan CafeMenuProject kodu çekiliyor..."
                git branch: 'main', url: "${GIT_REPO}"
            }
        }

        stage('Install Dependencies') {
            steps {
                echo "🔹 .NET bağımlılıkları yükleniyor..."
                sh 'dotnet restore Calia/Calia.sln'
            }
        }

        stage('Build Project for CategoryApi') {
            steps {
                echo "🔹 .NET 8 projesi derleniyor..."
                sh 'dotnet build Calia/Calia.Services.CategoryAPI/Calia.Services.CategoryAPI.csproj --configuration Release'
            }
        }

       stage('SonarQube Analysis for CategoryApi') {
            steps {
                script {
                    def scannerHome = tool 'SonarScanner for MSBuild'  
                    withSonarQubeEnv('sonarqube') {  
                        dir('Calia/Calia.Services.CategoryAPI') {
                            sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"CategoryApi\""
                            sh "dotnet build"
                            sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"  
                        }
                    }
                }
            }
        }

        stage('Quality Gate Check') {
            steps {
                echo "🔹 SonarQube Quality Gate kontrol ediliyor..."
                timeout(time: 10, unit: 'MINUTES') {
                    script {
                        def qg = waitForQualityGate()
                        if (qg.status != 'OK') {
                            error "❌ Quality Gate başarısız: ${qg.status}"
                        }
                    }
                }
            }
        }

        stage('Build Docker Image for CategoryApi') {
            steps {
                echo "🔹 Docker resmi oluşturuluyor..."
                sh 'docker build -t kerem1406/categoryapi:latest -f Calia/Calia.Services.CategoryAPI/Dockerfile Calia'
            }
        }

        stage('Push Docker Image to DockerHub') {
            steps {
                echo "🔹 Docker resmi DockerHub'a gonderiliyor..."
                sh 'docker push kerem1406/categoryapi:latest'
                sh "docker tag kerem1406/categoryapi:latest kerem1406/categoryapi:1.0.$BUILD_NUMBER"
                sh "docker push kerem1406/categoryapi:1.0.$BUILD_NUMBER"
            }
        }


    }
}
