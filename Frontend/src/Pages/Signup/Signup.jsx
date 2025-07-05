import { useContext, useState } from "react";
import InputField from "./Components/Input";
import { Link, useNavigate } from "react-router-dom";
import AuthContext from "../../Context/AuthContext";
import NetworkError from "../../Exceptions/NetworkError";
import BadRequest from "../../Exceptions/BadRequest";

export default function Signup() {
  const { signup } = useContext(AuthContext);
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [displayName, setDisplayName] = useState("");

  const [emailMessage, setEmailMessage] = useState("");
  const [usernameMessage, setUsernameMessage] = useState("");
  const [passwordMessage, setPasswordMessage] = useState("");
  const [confirmPasswordMessage, setConfirmPasswordMessage] = useState("");
  const [displayNameMessage, setDisplayNameMessage] = useState("");

  const navigate = useNavigate();
  const clearMessage = function () {
    setEmailMessage("");
    setUsernameMessage("");
    setPasswordMessage("");
    setConfirmPasswordMessage("");
    setDisplayNameMessage("");
  };
  async function handleSubmit(e) {
    e.preventDefault();
    if (password !== confirmPassword) {
      setConfirmPasswordMessage("Password not matched");
      return;
    }
    try {
      await signup({
        DisplayName: displayName,
        Email: email,
        Username: username,
        Password: password,
      });
      navigate("/login");
    } catch (e) {
      if (e instanceof NetworkError) {
        setEmailMessage(e.message);
        return;
      }
      if (e instanceof BadRequest) {
        let errorData = e.data;
        if (!errorData) {
          setDisplayNameMessage("Something went wrong");
          return;
        }
        let errors = errorData.errors;
        if (!errors) {
          setDisplayNameMessage(e.message);
          return;
        }
        console.log(Array.isArray(errors.DisplayName));

        "DisplayName" in errors && Array.isArray(errors.DisplayName)
          ? setDisplayNameMessage(errors.DisplayName[0])
          : "";
        "Email" in errors && Array.isArray(errors.Email)
          ? setEmailMessage(errors?.Email[0])
          : "";
        "Password" in errors && Array.isArray(errors.Password)
          ? setPasswordMessage(errors?.Password[0])
          : "";
        "UserName" in errors && Array.isArray(errors.UserName)
          ? setUsernameMessage(errors?.UserName[0])
          : "";

        return;
      }
      console.error(e);
    }
  }

  return (
    <div>

      <div className="mt-4 prose prose-h2:text-base-content prose-h2:text-3xl prose-h2:font-semibold flex flex-col prose-h2:text-center  w-full mx-auto items-center">
        <h2>Sign up for Chatly</h2>
        <form
          className="flex flex-col w-full gap-5"
          onSubmit={(e) => handleSubmit(e)}
        >
          <div className="flex flex-col w-full">
            <InputField
              legend="Name"
              placeholder="Enter your Name"
              value={displayName}
              message={displayNameMessage}
              type={"text"}
              onChange={(e) => {
                setDisplayName(e.target.value);
                clearMessage();
              }}
              required={true}
            />

            <InputField
              legend="Email"
              placeholder="Enter your email"
              value={email}
              message={emailMessage}
              type={"email"}
              onChange={(e) => {
                setEmail(e.target.value);
                clearMessage();
              }}
              required={true}
            />
            <InputField
              legend="Username"
              placeholder="Must be unique"
              value={username}
              message={usernameMessage}
              onChange={(e) => {
                setUsername(e.target.value);
                clearMessage();
              }}
              type={"text"}
              required={true}
            />
            <div className="flex gap-8">
              <InputField
                legend="Password"
                placeholder="Create a passwrod"
                value={password}
                message={passwordMessage}
                onChange={(e) => {
                  setPassword(e.target.value);
                  clearMessage();
                }}
                type={"password"}
                required={true}
              />
              <InputField
                legend="Confirm Password"
                placeholder="Confirm your Password"
                value={confirmPassword}
                message={confirmPasswordMessage}
                onChange={(e) => {
                  setConfirmPassword(e.target.value);
                  clearMessage();
                }}
                type={"password"}
                required={true}
              />
            </div>
          </div>
          <div className="w-full flex items-center flex-col gap-1">
            <button className="btn rounded-xl bg-accent text-base-300 dark:text-base-300 btn-block">
              Sign Up
            </button>
            <div className="prose prose-a:font-thin prose-a:no-underline prose-a:cursor-pointer prose-a:hover:underline prose-a:text-sm">
              <Link to="/login">Already have an account? Log in</Link>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
