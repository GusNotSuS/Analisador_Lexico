import re


token_specs = [
    ('Comentario', r'//.*//'),
    ('Espacamento', r'\s+'),
    ('Comando_de_Movimento', r'move_up|move_down|move_left|move_right'),
    ('Comando_de_Acao', r'jump|attack|defend'),
    ('Estrutura_de_Controle', r'if|else|while|for'),
    ('Numerico', r'\d+'),
    ('Identificador', r'hero|enemy|treasure|trap'),
    ('Operador', r'[+\-*/]'),
    ('Operador_Logico', r'&&|\|\||!'),
    ('Parenteses', r'[()]'),
    ('Chaves', r'[{}]'),
    ('Desconhecido', r'[^\s]+')
]

token_regex = '|'.join(f'(?P<{nome}>{regex})' for nome, regex in token_specs)

def analisador_lexico(codigo):
    tokens = []
    errors = []
    for match in re.finditer(token_regex, codigo):
        kind = match.lastgroup
        value = match.group()
        if kind == 'Espacamento':
            continue  # Ignorar espaços
        elif kind == 'Comentarios':
            tokens.append((kind, value))  # Capturar comentário
        elif kind == 'Desconhecido':
            errors.append(f'Token desconhecido: {value}')
        else:
            tokens.append((kind, value))
    return tokens, errors


if __name__ == "__main__":
    while True:
        entrada = input("Digite um comando QuestLang (ou 'encerrar' para sair): ")
        if entrada.lower() == 'encerrar':
            break
        tokens, errors = analisador_lexico(entrada)
        if tokens:
            print("Tokens reconhecidos:", tokens)
        if errors:
            print("Erros encontrados:", errors)

