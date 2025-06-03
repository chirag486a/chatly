function InputField({ legend, placeholder, message }) {
  return (
    <fieldset className="fieldset w-fit prose prose-p:m-0">
      <div>
        <p className="label opacity-100 text-error">{message ? message : ""}</p>
        <legend className="fieldset-legend text-base-content text-base font-normal pt-0">
          {legend}
        </legend>
      </div>
      <input
        type="text"
        className="input input-content input-lg rounded-xl text-base bg-transparent dark:text-content w-80"
        placeholder={placeholder}
      />
    </fieldset>
  );
}
export default function Settings() {
  return (
    <div className="px-6 py-12">
      <div className="w-8/12 mx-auto">
        <div className="prose prose-p:text-3xl prose-p:font-semibold prose-p:mb-8">
          <p>Settings</p>
        </div>
        <div className=" flex flex-col gap-8">
          <form>
            <div className="prose prose-p:text-lg prose-p:font-normal">
              <p>Edit Profile</p>
            </div>
            <InputField legend="Username" placeholder="@username" />
            <InputField legend="Email" placeholder="johndoe@mail.com" />
            <InputField legend="Display Name" placeholder="John Doe" />

            <button className="btn rounded-xl mt-8">
              Save Changes
            </button>
          </form>
          <div>
            <div className="prose prose-p:text-lg prose-p:font-normal">
              <p>Change Theme</p>
            </div>
            <div className="flex gap-6">
              <button className="btn btn-outline rounded-xl mt-8">
                Light Theme
              </button>
              <button className="btn btn-outline rounded-xl dark:text-base-300 mt-8">
                Dark Theme
              </button>
            </div>
          </div>
          <div>
            <div className="prose prose-p:text-lg prose-p:font-normal">
              <p>Delete Account</p>
            </div>
            <div className="flex gap-6">
              <button className="btn btn-error rounded-xl dark:text-base-300 mt-8">Delete My Account</button>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}
