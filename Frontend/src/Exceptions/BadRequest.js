class BadRequest extends Error {
  constructor(message, data, { cause } = {}) {
    super(message); // sets the message on the base Error class
    this.name = "";
    this.data = data;
    this.code = "ERR_BAD_REQUEST";
    this.cause = cause;
  }
}

export default BadRequest;
