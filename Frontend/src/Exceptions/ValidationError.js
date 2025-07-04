class ValidationError extends Error {
  constructor(message, field) {
    super(message); // sets the message on the base Error class
    this.name = "ValidationError";
    this.field = field;
    this.code = "INVALID_INPUT";
  }
}

export default ValidationError;
