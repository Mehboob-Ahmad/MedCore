const fs = require('fs');
const path = require('path');

function walkDir(dir) {
    const files = fs.readdirSync(dir);
    for (const file of files) {
        const fullPath = path.join(dir, file);
        if (fs.statSync(fullPath).isDirectory()) {
            walkDir(fullPath);
        } else if (fullPath.endsWith('.tsx') || fullPath.endsWith('.ts')) {
            let content = fs.readFileSync(fullPath, 'utf8');
            let modified = false;
            
            // Replace relative imports to components/ui with @medichp/ui
            // Matches: import { X } from "../../components/ui/Y"
            // Or: import { X } from "../../../../components/ui/Y"
            const regex = /import\s+{([^}]+)}\s+from\s+['"](?:\.\.\/)+components\/ui\/[^'"]+['"]/g;
            
            if (regex.test(content)) {
                content = content.replace(regex, (match, imports) => {
                    return `import { ${imports.trim()} } from "@medichp/ui"`;
                });
                modified = true;
            }

            if (modified) {
                fs.writeFileSync(fullPath, content);
                console.log(`Updated ${fullPath}`);
            }
        }
    }
}

walkDir(path.join(__dirname, 'apps', 'web', 'src', 'app'));
