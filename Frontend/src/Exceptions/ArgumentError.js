class ArgumentError extends Error {
  constructor(message, data, { cause } = {}) {
    super(message); // sets the message on the base Error class
    this.name = "";
    this.data = data;
    this.code = "ERR_ARGUMENT";
    this.cause = cause;
  }
}

export default ArgumentError;
