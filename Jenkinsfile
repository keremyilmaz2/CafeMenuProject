pipeline {
    agent any

    stages {
        stage('Run Allll Jobsss') {
            steps {
                script {
                    // Paralel olarak çalışacak tüm işleri tanımlıyoruz.
                    def jobs = [
                        order: {
                            echo 'Running Order Job...'
                            build job: 'Order' // Order işini tetikle
                        },
                        stock: {
                            echo 'Running Stock Job...'
                            build job: 'stock' // Stock işini tetikle
                        },
                        shoppingcart: {
                            echo 'Running ShoppingCart Job...'
                            build job: 'shoppingcart' // ShoppingCart işini tetikle
                        },
                        category: {
                            echo 'Running Category Job...'
                            build job: 'category' // Category işini tetikle
                        },
                        auth: {
                            echo 'Running Auth Job...'
                            build job: 'Auth' // Auth işini tetikle
                        },
                        product: {
                            echo 'Running Product Job...'
                            build job: 'Product' // Product işini tetikle
                        },
                        web: {
                            echo 'Running Web Job...'
                            build job: 'Web' // Web işini tetikle
                        }
                    ]
                    
                    // Paralel işleri çalıştırıyoruz.
                    parallel jobs
                }
            }
        }

        stage('Final Stage') {
            steps {
                echo 'All jobs are triggered and completed.'
            }
        }
    }
}
