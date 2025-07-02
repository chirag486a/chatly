import { Link } from "react-router-dom";
import InputField from "./Components/Input";
import AuthContext from "../../Context/AuthContext";
import { useContext, useEffect } from "react";

export default function Login() {
  const { login } = useContext(AuthContext);
  useEffect(() => {
    (async function () {
      var response = await login({
        Email: "chirag@gmail.com",
        Password: "Admin@123",
      });
      console.log(response)
    })();
  });
  return (
    <div>
      <div className="mt-8 prose prose-h2:text-base-content prose-h2:mb-2 prose-h2:text-3xl prose-h2:font-semibold flex flex-col gap-2 prose-h2:text-center  w-full mx-auto items-center">
        <h2>Welcome back</h2>
        <div className="prose prose-p:text-sm">
          <p>Signin to continue</p>
        </div>
        <form className="flex flex-col items-center w-fit gap-5">
          <div className="flex flex-col items-center w-fit">
            <InputField legend="Email" placeholder="Email" />
            <InputField legend="Password" placeholder="Password" />
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
