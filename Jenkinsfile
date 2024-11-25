pipeline{
agent any
stages {
        stage('Checkout') {
            steps {
               git branch: 'main', credentialsId: 'RISE', url: 'git@github.com:HOGENT-RISE/dotnet-2425-gent4.git'
            }
        }

stage("build") {
            
            steps {
                echo "build the app"
                script {
                
                    sh 'dotnet restore'
                    sh 'dotnet build'
         
                }
            }
        }
stage("test") {
            
            steps {
                echo "test the app"
                script {
                    sh 'dotnet test'
                }
            }
        }   
stage("publish") {
            
            steps {
                echo "publish the app"
                script {
                    sh 'dotnet publish -c Release -o ./publish '
                }
            }
        }        
   }      
} 
