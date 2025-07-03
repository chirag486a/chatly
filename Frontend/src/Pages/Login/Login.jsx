import { Link, useNavigate } from "react-router-dom";
import InputField from "./Components/Input";
import AuthContext from "../../Context/AuthContext";
import NetworkError from "../../Exceptions/NetworkError";
import { useContext, useState } from "react";
import BadRequest from "../../Exceptions/BadRequest";

export default function Login() {
  const { login, saveToken, saveUser } = useContext(AuthContext);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [emailMessage, setEmailMessage] = useState("");
  const navigate = useNavigate();

  async function handleSubmit(e) {
    try {
      e.preventDefault();
      var response = await login({
        Email: email,
        Password: password,
      });
      // BUG
      setEmail("");
      setPassword("");
      saveToken(response.data.token);
      saveUser(response.data.user);
      console.log("Logged in successfully");
      navigate("/");
    } catch (e) {
      if (e instanceof NetworkError) {
        setEmailMessage(e.message);
        return;
      }
      if (e instanceof BadRequest) {
        setEmailMessage(e.message);
        setPassword("");
        return;
      }
      console.error(e);
    }
  }
  return (
    <div>
      <div className="mt-8 prose prose-h2:text-base-content prose-h2:mb-2 prose-h2:text-3xl prose-h2:font-semibold flex flex-col gap-2 prose-h2:text-center  w-full mx-auto items-center">
        <h2>Welcome back</h2>
        <div className="prose prose-p:text-sm">
          <p>Signin to continue</p>
        </div>
        <form
          className="flex flex-col items-center w-fit gap-5"
          onSubmit={(e) => handleSubmit(e)}
        >
          <div className="flex flex-col items-center w-fit">
            <InputField
              legend="Email"
              placeholder="Email"
              onChange={(e) => {
                setEmail(e.target.value);
                setEmailMessage("");
              }}
              value={email}
              message={emailMessage}
              type={"email"}
              required={true}
            />
            <InputField
              legend="Password"
              placeholder="Password"
              onChange={(e) => {
                setPassword(e.target.value);
                setEmailMessage("");
              }}
              value={password}
              type={"password"}
              required={true}
            />
          </div>
          <div className="w-full flex items-center flex-col gap-1">
            <button className="btn rounded-xl bg-accent text-base-300 dark:text-base-300 btn-block">
              Sign Up
            </button>
            <div className="prose prose-a:font-thin prose-a:no-underline prose-a:cursor-pointer prose-a:hover:underline prose-a:text-sm">
              <Link to="/signup">Don't have an account? Sign up</Link>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
