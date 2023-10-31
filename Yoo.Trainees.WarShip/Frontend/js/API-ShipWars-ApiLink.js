const enviroments = { localhost: "127.0.0.1", dev: "yoo-shipwars-web-dev.azurewebsites.net"};
const hostname = window.location.hostname;
const env = getEnvByHostname(hostname);

const api = getApiUrlByEnv(env);

function getEnvByHostname(hostname) {
  const envIndex = Object.values(enviroments).findIndex(env => env === hostname);
  if (envIndex === -1) {
    throw new Error("Unknown hostname");
  }
  return Object.keys(enviroments)[envIndex];
}

function getApiUrlByEnv(env) {
  if (!Object.keys(enviroments).includes(env)) {
    throw new Error("Unknown enviroment");
  }
  const baseUrl = enviroments[env];
  return `https://${baseUrl}/api/Game`.replace("web", "api");
}