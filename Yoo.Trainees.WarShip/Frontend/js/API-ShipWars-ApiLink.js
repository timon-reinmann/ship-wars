const enviroments = { localhost: "127.0.0.1", dev: "yoo-shipwars-web-dev.azurewebsites.net"};
const hostname = window.location.hostname;
const env = getEnvByHostname(hostname);

const LOCALHOST_IP = "127.0.0.1";

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
  if(baseUrl === LOCALHOST_IP){
    return `https://localhost:7118/api/Game/`;
  }
  return `https://${baseUrl}/api/Game/`.replace("web", "api");
}