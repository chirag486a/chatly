import InputField from "./Components/Input";
import { Link } from "react-router-dom";

export default function Signup() {
  return (
    <div>
      <div className="mt-8 prose prose-h2:text-base-content prose-h2:text-3xl prose-h2:font-semibold flex flex-col prose-h2:text-center  w-full mx-auto items-center">
        <h2>Sign up for Chatly</h2>
        <form className="flex flex-col items-center w-fit gap-5">
          <div className="flex flex-col items-center w-fit">
            <InputField legend="Email" placeholder="Enter your email" />
            <InputField legend="Username" placeholder="Enter your username" />
            <InputField legend="Password" placeholder="Create a passwrod" />
            <InputField
              legend="Confirm Password"
              placeholder="Confirm your Password"
            />
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
