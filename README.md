# Berkeley-project-AGJM

Equipe:
André Luiz de Souza Nunes  
Grégori Pintarelli  
Jaqueline Klegin  
Matheus Eduardo dos Santos  

TRABALHO PRÁTICO 02 – SINCRONIZAÇÃO DE RELÓGIOS / COMUNICAÇÃO           

O algoritmo de Berkeley é o método que faz o relógio distribuir e sincronizar computadores. Ele faz uma consulta em cada computador e verifica os valores dos relógios. Efetua uma média dos dados coletados e informa a cada máquina para que se ajuste. Atrasando ou adiantando. 
Passos do funcionamento de Berkeley:
1.	Servidor solicita a hora dos clientes
2.	Cada cliente responde ao servidor informando qual é a diferença de tempo em relação a ele
3.	O servidor efetua a média dos tempos (incluindo a leitura dele).
4.	O servidor encaminha o ajuste necessário a ser feito pelo cliente (média + inversão da diferença de tempo enviada no passo 2).
5.	Cliente realiza o ajuste.
A avaliação observará aos seguintes critérios:
- 	implementação do algoritmo de Berkeley (4,0 pontos)
-   utilização comunicação entre serviços (tcp, udp, socket, rpc) (4,0 pontos)
- 	funcionamento correto (2,0 pontos)
