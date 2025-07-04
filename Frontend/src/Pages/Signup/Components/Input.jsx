export default function InputField({
  legend,
  placeholder,
  message,
  onChange,
  type,
  required,
  value,
}) {
  return (
    <fieldset className="fieldset w-full prose prose-p:m-0 max-w-full">
      <div>
        <p className="label opacity-100 text-error">{message ? message : ""}</p>
        <legend className="fieldset-legend text-base-content text-base font-normal pt-0">
          {legend}
        </legend>
      </div>
      <input
        type={type}
        className="input input-content input-lg rounded-xl text-base bg-transparent dark:text-content w-full"
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange(e)}
        required={required}
      />
    </fieldset>
  );
}
