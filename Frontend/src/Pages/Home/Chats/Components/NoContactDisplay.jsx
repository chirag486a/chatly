import PersonSittingInSofa from "../../../../assets/person-sitting-in-sofa.svg?react";
export default function NoContactDisplay() {
  return (
    <div className="w-full mt-8 flex flex-col items-center justify-center">
      <PersonSittingInSofa className="w-full" />
      <div className="prose-h3:text-lg prose-h3:font-semibold text-center prose-h3:leading-12 mt-4">
        <h3>No recent contacts.</h3>
        <p>Start a conversation by searching for users.</p>
      </div>
    </div>
  );
}
