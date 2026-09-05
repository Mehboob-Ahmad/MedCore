const fs = require('fs'); 
const code = fs.readFileSync('apps/web/src/app/patient/profile/page.tsx', 'utf8'); 
let clean = code.replace(/\/\*[\s\S]*?\*\//g, '').replace(/\/\/.*$/gm, '').replace(/\"[^\"]*\"/g, '\"\"').replace(/\'[^\']*\'/g, '\'\'').replace(/\`[^\`]*\`/g, '\`\`'); 
let open = 0; 
let lines = clean.split('\n'); 
let lineOpen = []; 
for(let i=0; i<lines.length; i++) { 
  let line = lines[i]; 
  for(const char of line) { 
    if(char==='{') { open++; lineOpen.push(i+1); } 
    if(char==='}') { open--; lineOpen.pop(); } 
  } 
} 
console.log('Unclosed at line:', lineOpen);
