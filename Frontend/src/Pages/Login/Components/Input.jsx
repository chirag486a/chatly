export default function InputField({ legend, placeholder, message }) {
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
