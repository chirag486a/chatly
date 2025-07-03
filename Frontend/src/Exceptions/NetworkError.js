class NetworkError extends Error {
  constructor(message, { cause } = {}) {
    super(message); // sets the message on the base Error class
    this.name = "NetworkError";
    this.code = "NETWORK_ERROR";
    this.cause = cause;
  }
}

export default NetworkError;
