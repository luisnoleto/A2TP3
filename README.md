# A2TP3 - Biblioteca - Emprestimo de Livros
# Atenção - Para o login ser feito corretamente: criar usuario no post de Usuario, fazer login no auth e copiar o JWT. No campo do Authorize colocar Bearer e o JWT (Ex Bearer ey0bs...).

Endpoints "publicos" - getCategorias, getLivros e postUsuario
O restante dos endpoints so pode ser utilizado por um usuario logado, inclusive para fazer os emprestimos e salvar no seu usuario.
Uma pessoa pode consultar os livros cadastrados, pode criar a conta e fazer login, cadastrar categorias para poder cadastrar livros, e enfim fazer um emprestimo, com a data de devolução colocada automaticamente com + 7 dias
Relações Usuario (ApplicationUser) 1:N Emprestimos, Emprestimos: N:N Livros, Livro N:1 Categoria, EmprestimoLivro N:1 Emprestimo, EmprestimoLivro N:1 Livro,
Não foi feito o endpoint de carrinho por ser uma feature que so tem um objetivo no front-end.
ConnectionString do banco de dados local usando InMemory
