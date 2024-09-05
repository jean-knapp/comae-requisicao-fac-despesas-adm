# Instruções para as UG EXEC
## Instruções de instalação
* Baixe a pasta compactada _programa.zip_ no link [[https://www.google.com](https://github.com/jean-knapp/comae-requisicao-fac-despesas-adm/releases/latest)]([programa.zip](https://github.com/jean-knapp/comae-requisicao-fac-despesas-adm/releases/latest)) e extraia todos os arquivos para um local do computador que haja permissão para ler/escrever.

## Requisitos
* Windows 7, 8, 10, 11
* Net Framework 4.7.2

# Instruções para o COMAE
## Estrutura do CSV

Cada arquivo CSV equivale a uma solicitação da UG EXEC e cada linha do arquivo representa uma despesa administrativa informada na solicitação. 

As informações são precedidas pelos dados do solicitante, que são repetidas em todas as linhas, e na sequência aparecem os dados da despesa administrativa.

As células são separadas por <kbd>;</kbd> e há um cabeçalho com os mesmos campos configurados no arquivo **config.kv**.

### Exemplo Completo de Arquivo csv
```csv
UGR Requisitante;Posto/Graduação;Nome de Guerra;Função na Operação;Telefone;E-mail;Tipo de Solicitação;Natureza de Despesa;Moeda;Valor;Memória de Cálculo;UG EXEC;Confirmação de Despesa Administrativa;Observações adicionais
COMAE;CP;Knapp;A-10;(84) 00000-0000;knapp@knapp.com;Descentralização;Diárias militares;Real;1000,00;0;COMAE;Sim;"Teste com ""citação"""
COMAE;CP;Knapp;A-10;(84) 00000-0000;knapp@knapp.com;Recolhimento;Material Permanente;Dólar americano;20000000000,00;sgaasggas;COMAE;Sim;
COMAE;CP;Knapp;A-10;(84) 00000-0000;knapp@knapp.com;Descentralização;Passagem;Real;500,50;"Memória detalhada";COMAE;Sim;Nenhuma observação.
COMAE;CP;Knapp;A-10;(84) 00000-0000;knapp@knapp.com;Recolhimento;Serviços de Terceiros;Peso;300,75;"Memória explicativa";COMAE;Sim;"Observações com ""aspas"" duplas"
COMAE;CP;Knapp;A-10;(84) 00000-0000;knapp@knapp.com;Descentralização;Material de consumo;Real;1200,00;"Cálculo completo";COMAE;Sim;"Testando observações"
COMAE;CP;Knapp;A-10;(84) 00000-0000;knapp@knapp.com;Recolhimento;Diárias militares;Dólar americano;1500,00;"Explicação da memória";COMAE;Sim;
```

## Estrutura do config.kv

Este documento descreve como estruturar o arquivo KeyValue utilizado para o sistema de Solicitação de Despesas Administrativas da FAC. O arquivo KeyValue contém configurações para dados do solicitante e dos itens da requisição. Siga este guia para garantir que os dados sejam lidos corretamente pelo sistema.

### Estrutura Geral

O arquivo KeyValue segue a seguinte estrutura geral:

- **Title**: Título do sistema.
- **WelcomeMessage**: Mensagem de boas-vindas exibida na interface do sistema.
- **Operations**: Lista de operações militares disponíveis.
- **Requester**: Seção contendo as opções para dados do solicitante.
- **RequestItem**: Seção contendo as opções para dados dos itens de despesa da requisição.

### Exemplo Completo de Arquivo KeyValue

Abaixo está um exemplo completo de como estruturar o arquivo KeyValue. Cada campo é explicado em detalhes nas seções subsequentes.

```groovy
DespesasAdministrativas
{
    Title "Requisição FAC para custeio de Despesas Administrativas"
    WelcomeMessage "Bem-vindo ao Sistema de Solicitação de Despesas Administrativas da FAC."

    Operations {
        "0" "Ágata Amazônia"
        "1" "Ágata Oeste"
        "2" "Catrimani II"
        "3" "Pantanal II"
    }

    Requester {
        Option {
            Name "UGR Requisitante"
            FieldType "Selectable"
            Preview "1"
            Descrption ""
            Items {
                "0" "COMAE"
            }
        }
        Option {
            Name "Posto/Graduação"
            FieldType "Selectable"
            Preview "1"
            Descrption ""
            Items {
                "0" "1T"
                "1" "2T"
                "2" "CP"
                "3" "MJ"
                "4" "TC"
                "5" "CL"
            }
        }
        Option {
            Name "Nome de Guerra"
            FieldType "RequiredText"
            Preview "1"
            Descrption ""
        }
        Option {
            Name "Função na Operação"
            FieldType "Selectable"
            Preview "1"
            Descrption ""
            Items {
                "0" "A-10"
            }
        }
        Option {
            Name "Telefone"
            FieldType "Phone"
            Preview "0"
            Descrption ""
        }
        Option {
            Name "E-mail"
            FieldType "RequiredText"
            Preview "0"
            Descrption ""
        }
    }

    RequestItem {
        Option {
            Name "Tipo de Solicitação"
            FieldType "Selectable"
            Preview "1"
            Description ""
            Items {
                "0" "Descentralização"
                "1" "Recolhimento"
            }
        }
        Option {
            Name "Natureza de Despesa"
            FieldType "Selectable"
            Preview "1"
            Description ""
            Items {
                "0" "Diárias militares"
                "1" "Passagem"
                "2" "Material de consumo"
                "3" "Material Permanente"
                "4" "Serviços de Terceiros"
            }
        }
        Option {
            Name "Moeda"
            FieldType "Selectable"
            Preview "1"
            Description ""
            Items {
                "0" "Real"
                "1" "Dólar americano"
                "2" "Peso"
            }
        }
        Option {
            Name "Valor"
            FieldType "Currency"
            Preview "1"
            Description "Informar o valor total calculado de demanda de descentralização ou de recolhimento."
        }
        Option {
            Name "Memória de Cálculo"
            FieldType "RequiredText"
            Preview "0"
            Description ""
        }
        Option {
            Name "UG EXEC"
            FieldType "Selectable"
            Preview "1"
            Description "Atenção!\nNesse campo deve ser informada a UG EXEC apoiadora e não a requisitante, conforme objeto de custeio."
            Items {
                "0" "COMAE"
            }
        }
        Option {
            Name "Confirmação de Despesa Administrativa"
            FieldType "Selectable"
            Preview "0"
            Description "Não deve ser solicitado o custeio de demandas de cunho logístico, tais como:\n* Suprimentos, combustíveis e lubrificantes de Aviação; e\nManutenções programadas que compõem o custo logístico da hora de voo."
            Items {
                "0" "Sim"
            }
        }
        Option {
            Name "Observações adicionais"
            FieldType "FreeText"
            Preview "0"
            Description "Campo facultativo."
        }
    }
}
```

### WelcomeMessage
A mensagem de boas-vindas pode incluir quebras de linha usando <kbd>\n</kbd>. Exemplo:

```groovy
WelcomeMessage "Bem-vindo\nao Sistema de Solicitação de Despesas Administrativas da FAC."
```

### Operations
A seção '''Operations''' define uma lista de operações disponíveis. Cada operação é identificada por um número e seu nome.

```groovy
Operations {
    "0" "Ágata Amazônia"
    "1" "Ágata Oeste"
    "2" "Catrimani II"
    "3" "Pantanal II"
}
```

### FieldType
O software dispõe das seguintes opções de **FieldType**:
* RequiredText: Texto livre.
* Selectable: Seleção entre as opções listadas em **Items**.
* Currency: Número decimal com duas casas e virgula como separador decimal.
* Phone: Formato (00) 00000-0000 ou (00) 0000-0000.
* * FreeText: Texto livre, opcional.

### Requester
A seção **Requester** contém os dados do solicitante. Cada **Option** define um campo que o solicitante deve preencher.

Estrutura de **Option**
* Name: Nome do campo.
* FieldType: Tipo de campo (e.g., Selectable, RequiredText, Phone).
* Preview: Define se o campo aparece em visualizações rápidas. (1 para sim, 0 para não).
* Description: Descrição ou instruções adicionais para o campo.
* Items: Opcional. Define as opções de seleção para campos do tipo '''Selectable'''.

Caso haja campos com **FieldType** _Selectable_, as opções devem ser listadas em **Items**.

Exemplo de Campos:
```groovy
Requester {
    Option {
        Name "UGR Requisitante"
        FieldType "Selectable"
        Preview "1"
        Descrption ""
        Items {
            "0" "COMAE"
        }
    }
}
```

### RequestItem
A seção **RequestItem** contém os itens da requisição de despesa. Sua estrutura é similar à da seção **Requester**, com campos específicos para os itens da solicitação de despesas administrativas.

Exemplo de Campos:
```groovy
RequestItem {
    Option {
        Name "Tipo de Solicitação"
        FieldType "Selectable"
        Preview "1"
        Description ""
        Items {
            "0" "Descentralização"
            "1" "Recolhimento"
        }
    }
}
```

### Instruções Gerais
* Nomes e Tipos de Campo: Certifique-se de que os campos estão com os tipos e nomes corretos para evitar erros na leitura do arquivo.
* Quebras de Linha: Utilize \n para inserir quebras de linha no texto.
* Escapando Caracteres: Utilize aspas duplas para strings. Se o valor conter aspas duplas, escape-as com \".

Siga este formato para garantir que o arquivo KeyValue seja lido corretamente pelo sistema.
