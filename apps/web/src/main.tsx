import React, { useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import './style.css';
type Task = { id:string; title:string; status:string };
const faces: Record<string,string> = {idle:'◕‿◕',happy:'≧◡≦',thinking:'◔_◔',curious:'◉_◉',alert:'⊙﹏⊙',sleeping:'－_－'};
function App() {
 const [expression,setExpression]=useState('idle');
 const [tasks,setTasks]=useState<Task[]>([]);
 const [question,setQuestion]=useState(''); const [answer,setAnswer]=useState(''); const [error,setError]=useState('');
 const [busy,setBusy]=useState(false);
 async function reload(){try {const result=await fetch('/api/tasks'); if(!result.ok)throw new Error('API hors ligne'); setTasks(await result.json());}catch(e){setError(String(e));}}
 useEffect(()=>{void reload(); const timer=setInterval(()=>{void reload()},15000);return()=>clearInterval(timer);},[]);
 async function reactTo(face:string){try{const response=await fetch('/api/companion/expression',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({expression:face})});if(!response.ok) throw new Error('Expression indisponible');setExpression(face);}catch(e){setError(String(e));}}
 async function ask(event:React.FormEvent){event.preventDefault();if(!question.trim())return;setBusy(true);setAnswer('');setError('');void reactTo('thinking');try{const response=await fetch('/api/ai/chat',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({message:question})});if(!response.ok)throw new Error('Ollama indisponible : configure le serveur et le modèle');const data=await response.json();setAnswer(data.answer ?? 'Pas de réponse');void reactTo('happy');}catch(e){setError(String(e));void reactTo('alert');}finally{setBusy(false);}}
 return <main><header><span>FOX / CONTROL CENTER</span><span className="subtitle">Compagnon modulaire · Prototype v0.1</span></header><section className="grid"><article className="companion"><div className="ears">▲　▲</div><div className="face" aria-label={'expression '+expression}>{faces[expression]}</div><h1>Bonjour, développeur !</h1><p>État : {expression}</p><div className="actions">{['idle','happy','thinking','curious','alert','sleeping'].map(f=><button key={f} onClick={()=>void reactTo(f)}>{f}</button>)}</div></article><article><h2>Kanban local</h2><p className="muted">Données de démonstration en mémoire · intégration GitHub à venir</p>{tasks.map(t=><div className="task" key={t.id}><span>{t.title}</span><span className="pill">{t.status}</span></div>)}<h2>Assistant IA</h2><p className="muted">Ollama distant, sans RAG dans cette première version</p><form onSubmit={e=>void ask(e)}><input value={question} onChange={e=>setQuestion(e.target.value)} placeholder="Pose une question de code..."/><button disabled={busy}>{busy?'Réflexion...':'Envoyer'}</button></form>{answer && <div className="answer">{answer}</div>}{error&&<div className="error">{error}</div>}</article></section><footer>FOX · Open source ready · Hardware agnostic</footer></main>
}
createRoot(document.getElementById('root')!).render(<React.StrictMode><App/></React.StrictMode>);
