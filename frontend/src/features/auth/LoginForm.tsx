import { useState } from "react";
import { useToken } from "../../app/TokenContext";
import { useNavigate } from "react-router-dom";

export function LoginForm() {
  const { setToken } = useToken();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const API_BASE_URL: string =
      import.meta.env.VITE_API_URL || "http://localhost:5000";

    try {
      const res = await fetch(`${API_BASE_URL}/api/v1/login/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });
      if (!res.ok) throw new Error("Login failed!");
      const data = await res.json();
      setToken(data.token);

      navigate("/menu");
    } catch (err: any) {
      setError(err.message);
    }
  };
  return (
    <form onSubmit={handleSubmit}>
      <input
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        placeholder="Username"
      />
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
      />
      <button type="submit">Login</button>
      {error && <div style={{ color: "red" }}>{error}</div>}
    </form>
  );
}
