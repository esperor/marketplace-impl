const Profile = ({className}: {className?: string}) => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 20.05 20.05"
      strokeWidth={1.5}
      className={`size-4 stroke-slate-100 fill-none ${className}`}
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M 15.99 16.73 A 7.49 7.49 0 0 0 10.01 13.75 a 7.49 7.49 0 0 0 -5.98 2.98 m 11.96 0 a 9 9 0 1 0 -11.96 0 m 11.96 0 A 8.97 8.97 0 0 1 10.01 19 a 8.97 8.97 0 0 1 -5.98 -2.27 M 13.01 7.75 a 3 3 0 1 1 -6 0 a 3 3 0 0 1 6 0 Z"
      />
    </svg>
  );
};

export default Profile;
