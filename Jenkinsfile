@Library('jenkins-helpers@v0.1.10') _

def label = "cognite-net-sdk-${UUID.randomUUID().toString()}"

podTemplate(
    label: label,
    annotations: [
            podAnnotation(key: "jenkins/build-url", value: env.BUILD_URL ?: ""),
            podAnnotation(key: "jenkins/github-pr-url", value: env.CHANGE_URL ?: ""),
    ],
    containers: [containerTemplate(name: 'docker',
                                   command: '/bin/cat -',
                                   image: 'docker:17.06.2-ce',
                                   resourceRequestCpu: '1000m',
                                   resourceRequestMemory: '500Mi',
                                   resourceLimitCpu: '1000m',
                                   resourceLimitMemory: '500Mi',
                                   ttyEnabled: true),
                 containerTemplate(name: 'dotnet-mono',
                                   image: 'eu.gcr.io/cognitedata/dotnet-mono:2.2-sdk',
                                   envVars: [
                                             secretEnvVar(key: 'CODECOV_TOKEN', secretName: 'codecov-tokens', secretKey: 'cognite-sdk-net'),
                                             secretEnvVar(key: 'TEST_API_KEY_READ', secretName: 'fusiondotnet-sdk-api-keys', secretKey: 'publicdata'),
                                             secretEnvVar(key: 'TEST_API_KEY_WRITE', secretName: 'fusiondotnet-sdk-api-keys', secretKey: 'greenfield'),

                                             // /codecov-script/upload-report.sh relies on the following
                                             // Jenkins and Github environment variables.
                                             envVar(key: 'JENKINS_URL', value: env.JENKINS_URL),
                                             envVar(key: 'BRANCH_NAME', value: env.BRANCH_NAME),
                                             envVar(key: 'BUILD_NUMBER', value: env.BUILD_NUMBER),
                                             envVar(key: 'BUILD_URL', value: env.BUILD_URL),
                                             envVar(key: 'CHANGE_ID', value: env.CHANGE_ID),
                                   ],
                                   resourceRequestCpu: '1500m',
                                   resourceRequestMemory: '3000Mi',
                                   resourceLimitCpu: '1500m',
                                   resourceLimitMemory: '3000Mi',
                                   ttyEnabled: true)],
    volumes: [secretVolume(secretName: 'nuget-credentials',
                           mountPath: '/nuget-credentials',
                           readOnly: true),
              secretVolume(secretName: 'jenkins-docker-builder',
                           mountPath: '/jenkins-docker-builder',
                           readOnly: true),
              configMapVolume(configMapName: 'codecov-script-configmap', mountPath: '/codecov-script'),
              hostPathVolume(hostPath: '/var/run/docker.sock', mountPath: '/var/run/docker.sock'),
              configMapVolume(configMapName: 'mysql-test-scripts', mountPath: '/mysql-test-scripts', readOnly: true)]
) {
    properties([buildDiscarder(logRotator(daysToKeepStr: '30', numToKeepStr: '20'))])
    node(label) {
        def gitCommit
        def buildDirectory
        container('jnlp') {
            stage('Checkout') {
                checkout([$class: 'GitSCM',
                  branches: scm.branches,
                  extensions: [
                    [ $class: 'SubmoduleOption',
                      disableSubmodules: false,
                      parentCredentials: true,
                      recursiveSubmodules: true,
                      reference: '',
                      trackingSubmodules: false],
                    [ $class: 'CleanCheckout' ]
                  ],
                  userRemoteConfigs: scm.userRemoteConfigs
                ])

                gitCommit = sh(returnStdout: true, script: 'git rev-parse --short=12 HEAD').trim()
            }
        }

        container('dotnet-mono') {
          stage('Install dependencies') {
            sh('apt-get update && apt-get install -y libxml2-utils')
            sh('cp /nuget-credentials/nuget.config ./nuget.config')
            sh('./credentials.sh')
            sh('mono .paket/paket.exe install')
          }

          stage('Build') {
            sh('dotnet build')
          }

          stage('Run tests') {
            sh('./test.sh')

            archiveArtifacts artifacts: 'coverage.lcov', fingerprint: true
          }

          stage("Upload report to codecov.io") {
            sh('bash </codecov-script/upload-report.sh')
          }

          stage("Deploy package to registry") {
            if (env.BRANCH_NAME == 'master') {
              sh('./deploy.sh')
            }
          }
        }
    }
}
