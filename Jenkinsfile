pipeline {
    agent none
    stages {
        stage('Dotnet Test') {
            agent {
                node {
                    label 'builtin'
                }
            }
            steps {
                sh 'chmod +x ./old_scripts/dotnet_tests.sh'
                sh './old_scripts/dotnet_tests.sh'
            }
        }
        stage('Dotnet Run') {
            agent {
                node {
                    label 'dotnetserver'
                }
            }
            steps {
                sh 'chmod +x ./old_scripts/Dotnet_build_swarm.sh'
                sh './old_scripts/Dotnet_build_swarm.sh'
            }
        }
    }
}
