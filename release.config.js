const serviceName = 'auth'

class SemanticReleaseError extends Error {
    constructor(message, code, details) {
        super(message);
        Error.captureStackTrace(this, this.constructor);
        this.name = 'SemanticReleaseError';
        this.details = details;
        this.code = code;
        this.semanticRelease = true;
    }
}

module.exports = {
    verifyConditions: [
        () => {
            if (!process.env.DOCKERHUB_LOGIN) {
                throw new SemanticReleaseError(
                    "No DOCKERHUB_LOGIN specified",
                    "ENODOCKERHUB_LOGIN",
                    "Please make sure to add docker hub user in `DOCKERHUB_LOGIN` environment variable on your CI environment. The token must be able to push docker image");
            }
            if (!process.env.GH_TOKEN) {
                throw new SemanticReleaseError(
                    "No GH_TOKEN specified",
                    "ENOGH_TOKEN",
                    "Please make sure to github token in `GH_TOKEN` environment variable on your CI environment. The token must be able to create releases");
            }
            if (!process.env.DOCKERHUB_PASSWORD) {
                throw new SemanticReleaseError(
                    "No DOCKERHUB_PASSWORD specified",
                    "ENODOCKERHUB_PASSWORD",
                    "Please make sure to add docker password in `DOCKERHUB_PASSWORD` environment variable on your CI environment.");
            }
        },
        "@semantic-release/github"
    ],
    prepare: [
        {
            path: "@semantic-release/exec",
            cmd: "echo \"$DOCKERHUB_PASSWORD\" | docker login --username \"$DOCKERHUB_LOGIN\" --password-stdin"
        },
        {
            path: "@semantic-release/exec",
            cmd: `docker pull fossapps/micro.${serviceName}:\`git rev-parse --short=4 ${process.env.GITHUB_SHA}\``
        },
        {
            path: "@semantic-release/exec",
            cmd: `docker tag fossapps/micro.${serviceName}:\`git rev-parse --short=4 ${process.env.GITHUB_SHA}\` fossapps/micro.${serviceName}:\${nextRelease.version}`
        },
        {
            path: "@semantic-release/exec",
            cmd: `docker tag fossapps/micro.${serviceName}:\`git rev-parse --short=4 ${process.env.GITHUB_SHA}\` fossapps/micro.${serviceName}:latest`
        }
    ],
    publish: [
        {
            path: "@semantic-release/exec",
            cmd: `docker push fossapps/micro.${serviceName}:\${nextRelease.version}`
        },
        {
            path: "@semantic-release/exec",
            cmd: `docker push fossapps/micro.${serviceName}:latest`
        },
        "@semantic-release/github"
    ]
};
